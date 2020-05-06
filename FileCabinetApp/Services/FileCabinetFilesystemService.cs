using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetService with Filesystem.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int StringLength = 120;
        private const int OffsetId = sizeof(short);
        private const int OffsetFirstName = OffsetId + sizeof(int);
        private const int OffsetLastName = OffsetFirstName + StringLength;
        private const int OffsetDateOfBirth = OffsetLastName + StringLength;
        private const int OffsetGender = OffsetDateOfBirth + (3 * sizeof(int));
        private const int OffsetPassportId = OffsetGender + sizeof(char);
        private const int OffsetSalary = OffsetPassportId + sizeof(short);
        private const int RecordLength = OffsetSalary + sizeof(decimal);
        private readonly Dictionary<int, long> offsetById = new Dictionary<int, long>();

        private readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private readonly FileStream fileStream;
        private readonly IValidator validator;
        private readonly Encoding enc = Encoding.Unicode;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        /// <param name="validator">Validator for params.</param>
        /// <exception cref="ArgumentNullException">Throw when fileStream or validator is null.</exception>
        public FileCabinetFilesystemService(FileStream fileStream, IValidator validator)
        {
            if (fileStream is null)
            {
                throw new ArgumentNullException(nameof(fileStream), "FileStream can't be null");
            }

            if (validator is null)
            {
                throw new ArgumentNullException(nameof(validator), "Validator can't be null");
            }

            this.fileStream = fileStream;
            this.validator = validator;
        }

        /// <summary>
        /// Create a new record.
        /// </summary>
        /// <param name="recordData">User's data.</param>
        /// <returns>Id of a new record.</returns>
        /// <exception cref="ArgumentNullException">Throw when recordData is null.</exception>
        public int CreateRecord(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            this.validator.ValidateParameters(recordData);

            int id = recordData.Id;
            if (id < 1)
            {
                id = this.GetNextFreeId();
            }
            else
            {
                this.CheckId(id);
            }

            this.WriteToFile(recordData, id, this.offsetById[id]);

            return id;
        }

        /// <summary>
        /// Returns records.
        /// </summary>
        /// <param name="filter">Record's filter. Filter start from 'where' and can contain 'and' and 'or'.</param>
        /// <returns>Records by filret.</returns>
        /// <exception cref="ArgumentNullException">Throw when filter is null.</exception>
        public ReadOnlyCollection<FileCabinetRecord> SelectRecords(string filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            List<int> foundResult;
            if (filter.Length != 0)
            {
                string[] values = filter.Split(new string[] { " = '", " ='", "= '", "='", "' ", " " }, StringSplitOptions.RemoveEmptyEntries);
                values[^1] = values[^1][0..^1];
                if (values.Length % 3 != 0)
                {
                    throw new ArgumentException("Incorrect format", nameof(filter));
                }

                foundResult = this.Find(values).ToList();
            }
            else
            {
                foundResult = this.offsetById.Keys.ToList();
            }

            List<FileCabinetRecord> list = new List<FileCabinetRecord>();

            for (int i = 0; i < foundResult.Count; i++)
            {
                this.fileStream.Position = this.offsetById[foundResult[i]];
                FileCabinetRecord record = this.ReadFromFile();
                if (record != null)
                {
                    list.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(list);
        }

        /// <summary>
        /// Returns count of records  and count of deleted records.
        /// </summary>
        /// <returns>Count of records  and count of deleted records.</returns>
        public Tuple<int, int> GetStat()
        {
            long pos = this.fileStream.Position;
            int deletedCount = 0;
            int i = 0;
            while (i < this.fileStream.Length / RecordLength)
            {
                this.fileStream.Position = (i * RecordLength) + 1;
                int b = this.fileStream.ReadByte();
                if ((b & 2) == 2)
                {
                    deletedCount++;
                }

                i++;
            }

            this.fileStream.Position = pos;
            return new Tuple<int, int>((int)(this.fileStream.Length / RecordLength), deletedCount);
        }

        /// <summary>
        /// Create new snapshot.
        /// </summary>
        /// <returns>Snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            this.fileStream.Position = 0;
            for (int i = 0; i < this.fileStream.Length / RecordLength; i++)
            {
                FileCabinetRecord record = this.ReadFromFile();
                if (record != null)
                {
                    list.Add(record);
                }
            }

            return new FileCabinetServiceSnapshot(list.ToArray());
        }

        /// <summary>
        /// Restore date from snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot.</param>
        /// <exception cref="ArgumentNullException">Throw when snapshot is null.</exception>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot), "Snapshot can't be null");
            }

            List<FileCabinetRecord> list = snapshot.Records.ToList();
            foreach (FileCabinetRecord record in list)
            {
                long pos = this.fileStream.Length;
                try
                {
                    pos = this.offsetById[record.Id];
                }
                catch (KeyNotFoundException)
                {
                    this.offsetById.Add(record.Id, pos);
                }

                RecordData recordData = new RecordData(record.FirstName, record.LastName, record.DateOfBirth, record.Gender, record.PassportId, record.Salary);
                this.WriteToFile(recordData, record.Id, pos);
            }
        }

        /// <summary>
        /// Defragment the data file.
        /// </summary>
        /// <returns>Count of defragmented records.</returns>
        public int Purge()
        {
            int purgedCount = 0;
            int i = 0, offsetCount = 0;
            while (i < this.fileStream.Length / RecordLength)
            {
                this.fileStream.Position = (i * RecordLength) + 1;
                int b = this.fileStream.ReadByte();
                if ((b & 2) == 2)
                {
                    offsetCount++;
                    i++;
                }
                else
                {
                    if (offsetCount > 0)
                    {
                        this.DefragmentFile(i, offsetCount);
                        i -= offsetCount;
                        purgedCount += offsetCount;
                        offsetCount = 0;
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            this.fileStream.SetLength(this.fileStream.Length - (offsetCount * RecordLength));

            return purgedCount + offsetCount;
        }

        /// <summary>
        /// Delete record by parameters.
        /// </summary>
        /// <param name="param">Record parameters.</param>
        /// <returns>List of id recored, that was deleted.</returns>
        /// <exception cref="ArgumentNullException">Throw when param is null.</exception>
        /// <exception cref="ArgumentException">Throw when param not contains 'where', 'or', 'and' or have incorrect data.</exception>
        public IEnumerable<int> Delete(string param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            string[] values = param.Split(new string[] { " = '", " ='", "= '", "='", "' ", " " }, StringSplitOptions.RemoveEmptyEntries);
            values[^1] = values[^1][0..^1];
            if (values.Length % 3 != 0)
            {
                throw new ArgumentException("Incorrect format", nameof(param));
            }

            List<int> foundResult = this.Find(values).ToList();
            for (int i = 0; i < foundResult.Count; i++)
            {
                this.Remove(foundResult[i]);
                this.offsetById.Remove(foundResult[i]);
            }

            return foundResult;
        }

        /// <summary>
        /// Update records by parameters.
        /// </summary>
        /// <param name="param">Record parameters. Filter start from 'where' and can contain 'and' and 'or'.</param>
        /// <exception cref="ArgumentNullException">Throw when param is null.</exception>
        /// <exception cref="ArgumentException">Throw when param not contains 'where', 'or', 'and' or have incorrect data.</exception>
        public void Update(string param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            int whereIndex = param.IndexOf("where", StringComparison.Ordinal);
            if (whereIndex == -1)
            {
                throw new ArgumentException("Incorrect format", nameof(param));
            }

            string whereParams = param[whereIndex..];
            string[] whereValues = whereParams.Split(new string[] { " = '", " ='", "= '", "='", "' ", " " }, StringSplitOptions.RemoveEmptyEntries);
            whereValues[^1] = whereValues[^1][0..^1];
            if (whereValues.Length % 3 != 0)
            {
                throw new ArgumentException("Incorrect format", nameof(param));
            }

            List<int> foundResult = this.Find(whereValues).ToList();

            string setParams = param.Substring(4, whereIndex - 5);
            string[] setValues = setParams.Split(new string[] { " = '", " ='", "= '", "='", "' , ", "', ", "'" }, StringSplitOptions.RemoveEmptyEntries);
            if (setValues.Length % 2 != 0)
            {
                throw new ArgumentException("Incorrect format", nameof(param));
            }

            this.UpdateRecords(foundResult, setValues);
        }

        private static bool CheckOperator(string operatorSring, bool isEqual, bool isNeedAdd, int index)
        {
            if (isEqual)
            {
                if (operatorSring == "or")
                {
                    isNeedAdd = true;
                }
                else
                {
                    if (operatorSring != "and" && !(index == 0 && operatorSring == "where"))
                    {
                        throw new ArgumentException("Incorrect format", nameof(operatorSring));
                    }
                }
            }
            else
            {
                if (!(isNeedAdd && operatorSring == "or"))
                {
                    isNeedAdd = false;
                }
            }

            return isNeedAdd;
        }

        private void UpdateRecords(List<int> foundResult, string[] setValues)
        {
            for (int j = 0; j < foundResult.Count; j++)
            {
                for (int i = 0; i < setValues.Length; i += 2)
                {
                    RecordData recordData;
                    switch (setValues[i].ToUpperInvariant())
                    {
                        case "FIRSTNAME":
                            recordData = new RecordData() { FirstName = setValues[i + 1] };
                            this.validator.ValidatePrameter("firstname", recordData);
                            this.WriteFirstName(setValues[i + 1], this.offsetById[foundResult[j]]);
                            break;
                        case "LASTNAME":
                            recordData = new RecordData() { LastName = setValues[i + 1] };
                            this.validator.ValidatePrameter("lastname", recordData);
                            this.WriteLastName(setValues[i + 1], this.offsetById[foundResult[j]]);
                            break;
                        case "DATEOFBIRTH":
                            if (!DateTime.TryParse(setValues[i + 1], this.culture, DateTimeStyles.None, out DateTime dateOfBirth))
                            {
                                throw new ArgumentException("Incorrect dateofbith", nameof(setValues));
                            }

                            recordData = new RecordData() { DateOfBirth = dateOfBirth };
                            this.validator.ValidatePrameter("dateofbirth", recordData);
                            this.WriteDateOfBirth(dateOfBirth, this.offsetById[foundResult[j]]);
                            break;
                        case "GENDER":
                            if (!char.TryParse(setValues[i + 1], out char gender))
                            {
                                throw new ArgumentException("Incorrect gender", nameof(setValues));
                            }

                            recordData = new RecordData() { Gender = gender };
                            this.validator.ValidatePrameter("gender", recordData);
                            this.WriteGender(gender, this.offsetById[foundResult[j]]);
                            break;
                        case "PASSPORTID":
                            if (!short.TryParse(setValues[i + 1], out short passportId))
                            {
                                throw new ArgumentException("Incorrect passportId", nameof(setValues));
                            }

                            recordData = new RecordData() { PassportId = passportId };
                            this.validator.ValidatePrameter("passportid", recordData);
                            this.WritePassportId(passportId, this.offsetById[foundResult[j]]);
                            break;
                        case "SALARY":
                            if (!decimal.TryParse(setValues[i + 1], out decimal salary))
                            {
                                throw new ArgumentException("Incorrect salary", nameof(setValues));
                            }

                            recordData = new RecordData() { Salary = salary };
                            this.validator.ValidatePrameter("salary", recordData);
                            this.WriteSalary(salary, this.offsetById[foundResult[j]]);
                            break;
                        default: throw new ArgumentException("Incorrect format", nameof(setValues));
                    }
                }
            }
        }

        private bool Remove(int id)
        {
            long pos;
            try
            {
                pos = this.offsetById[id];
            }
            catch (KeyNotFoundException)
            {
                return false;
            }

            this.fileStream.Position = pos + 1;
            this.fileStream.WriteByte(0b10);
            this.offsetById.Remove(id);
            return true;
        }

        private void DefragmentFile(int i, int offsetCount)
        {
            while (i < this.fileStream.Length / RecordLength)
            {
                this.fileStream.Position = i * RecordLength;
                FileCabinetRecord record = this.ReadFromFile();
                RecordData recordData = new RecordData(record.FirstName, record.LastName, record.DateOfBirth, record.Gender, record.PassportId, record.Salary);
                this.WriteToFile(recordData, record.Id, (i - offsetCount) * RecordLength);
                i++;
            }

            this.fileStream.SetLength(this.fileStream.Length - (offsetCount * RecordLength));
        }

        private void WriteFirstName(string firstName, long pos)
        {
            this.fileStream.Position = pos + OffsetFirstName;
            byte[] tempFirstName = this.enc.GetBytes(firstName);
            Array.Resize(ref tempFirstName, StringLength);
            this.fileStream.Write(tempFirstName, 0, StringLength);
        }

        private void WriteLastName(string lastName, long pos)
        {
            this.fileStream.Position = pos + OffsetLastName;
            byte[] tempLastName = this.enc.GetBytes(lastName);
            Array.Resize(ref tempLastName, StringLength);
            this.fileStream.Write(tempLastName, 0, StringLength);
        }

        private void WriteDateOfBirth(DateTime dateOfBirth, long pos)
        {
            this.fileStream.Position = pos + OffsetDateOfBirth;
            this.fileStream.Write(BitConverter.GetBytes(dateOfBirth.Year), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(dateOfBirth.Month), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(dateOfBirth.Day), 0, sizeof(int));
        }

        private void WriteGender(char gender, long pos)
        {
            this.fileStream.Position = pos + OffsetGender;
            this.fileStream.Write(BitConverter.GetBytes(gender), 0, sizeof(char));
        }

        private void WritePassportId(short passportId, long pos)
        {
            this.fileStream.Position = pos + OffsetPassportId;
            this.fileStream.Write(BitConverter.GetBytes(passportId), 0, sizeof(short));
        }

        private void WriteSalary(decimal salary, long pos)
        {
            this.fileStream.Position = pos + OffsetSalary;
            int[] sal = decimal.GetBits(salary);
            foreach (var s in sal)
            {
                this.fileStream.Write(BitConverter.GetBytes(s), 0, sizeof(int));
            }
        }

        private void WriteToFile(RecordData recordData,  int id, long pos)
        {
            this.fileStream.Position = pos;

            this.fileStream.Write(BitConverter.GetBytes(0), 0, sizeof(short));
            this.fileStream.Write(BitConverter.GetBytes(id), 0, sizeof(int));
            byte[] tempFirstName = this.enc.GetBytes(recordData.FirstName);
            this.fileStream.Write(tempFirstName, 0, tempFirstName.Length);
            this.fileStream.Position += StringLength - tempFirstName.Length;
            byte[] tempLastName = this.enc.GetBytes(recordData.LastName);
            this.fileStream.Write(tempLastName, 0, tempLastName.Length);
            this.fileStream.Position += StringLength - tempLastName.Length;
            this.fileStream.Write(BitConverter.GetBytes(recordData.DateOfBirth.Year), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(recordData.DateOfBirth.Month), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(recordData.DateOfBirth.Day), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(recordData.Gender), 0, sizeof(char));
            this.fileStream.Write(BitConverter.GetBytes(recordData.PassportId), 0, sizeof(short));
            int[] sal = decimal.GetBits(recordData.Salary);
            foreach (var s in sal)
            {
                this.fileStream.Write(BitConverter.GetBytes(s), 0, sizeof(int));
            }

            this.fileStream.Flush();
        }

        private FileCabinetRecord ReadFromFile()
        {
            byte[] temp = new byte[RecordLength - (2 * StringLength)];
            FileCabinetRecord record = new FileCabinetRecord();

            this.fileStream.Read(temp, 0, sizeof(short));
            if ((temp[1] & 2) == 2)
            {
                this.fileStream.Position += RecordLength;
                return null;
            }

            this.fileStream.Read(temp, OffsetId, sizeof(int));
            record.Id = BitConverter.ToInt32(temp, OffsetId);

            byte[] tempForStrings = new byte[StringLength];
            this.fileStream.Read(tempForStrings, 0, StringLength);
            int end = 0;
            while (tempForStrings[end] != '\0' || tempForStrings[end + 1] != '\0')
            {
                end += 2;
            }

            Array.Resize(ref tempForStrings, end);
            record.FirstName = this.enc.GetString(tempForStrings);

            tempForStrings = new byte[StringLength];
            this.fileStream.Read(tempForStrings, 0, StringLength);
            end = 0;
            while (tempForStrings[end] != '\0' || tempForStrings[end + 1] != '\0')
            {
                end += 2;
            }

            Array.Resize(ref tempForStrings, end);
            record.LastName = this.enc.GetString(tempForStrings);

            this.fileStream.Read(temp, OffsetDateOfBirth - (2 * StringLength), sizeof(int));
            int year = BitConverter.ToInt32(temp, 6);

            this.fileStream.Read(temp, OffsetDateOfBirth - (2 * StringLength) + sizeof(int), sizeof(int));
            int month = BitConverter.ToInt32(temp, 10);

            this.fileStream.Read(temp, OffsetDateOfBirth - (2 * StringLength) + (2 * sizeof(int)), sizeof(int));
            int day = BitConverter.ToInt32(temp, 14);

            record.DateOfBirth = new DateTime(year, month, day);

            this.fileStream.Read(temp, OffsetGender - (2 * StringLength), sizeof(char));
            record.Gender = BitConverter.ToChar(temp, 18);

            this.fileStream.Read(temp, OffsetPassportId - (2 * StringLength), sizeof(short));
            record.PassportId = BitConverter.ToInt16(temp, 20);

            int[] bytes = new int[4];
            for (int j = 0; j < 4; j++)
            {
                this.fileStream.Read(temp, OffsetSalary - (2 * StringLength) + (j * sizeof(int)), sizeof(int));
                bytes[j] = BitConverter.ToInt32(temp, OffsetSalary - (2 * StringLength) + (j * sizeof(int)));
            }

            record.Salary = new decimal(bytes);

            return record;
        }

        private int GetNextFreeId()
        {
            int i = 1;
            bool isFound = false;
            while (!isFound)
            {
                try
                {
                    _ = this.offsetById[i];
                    i++;
                }
                catch (KeyNotFoundException)
                {
                    isFound = true;
                }
            }

            this.offsetById.Add(i, this.fileStream.Length);
            return i;
        }

        private void CheckId(int id)
        {
            try
            {
                long pos = this.offsetById[id];
                this.fileStream.Position = pos;
            }
            catch (KeyNotFoundException)
            {
                this.offsetById.Add(id, this.fileStream.Length);
                this.fileStream.Seek(0, SeekOrigin.End);
            }
        }

        private IEnumerable<int> Find(string[] values)
        {
            List<int> resultList = new List<int>();
            int j = 0;
            this.fileStream.Position = 0;
            while (j < this.fileStream.Length / RecordLength)
            {
                FileCabinetRecord record = this.ReadFromFile();
                bool isNeedAdd = true;
                for (int i = 0; i < values.Length; i += 3)
                {
                    switch (values[i + 1].ToUpperInvariant())
                    {
                        case "id":
                            if (!int.TryParse(values[i + 2], out int id))
                            {
                                throw new ArgumentException("Incorrect id", nameof(values));
                            }

                            isNeedAdd = CheckOperator(values[i], record.Id == id, isNeedAdd, i);
                            break;
                        case "FIRSTNAME":
                            isNeedAdd = CheckOperator(
                                values[i],
                                string.Equals(record.FirstName, values[i + 2], StringComparison.InvariantCultureIgnoreCase),
                                isNeedAdd,
                                i);
                            break;
                        case "LASTNAME":
                            isNeedAdd = CheckOperator(
                                values[i],
                                string.Equals(record.LastName, values[i + 2], StringComparison.InvariantCultureIgnoreCase),
                                isNeedAdd,
                                i);
                            break;
                        case "DATEOFBIRTH":
                            isNeedAdd = CheckOperator(
                                values[i],
                                string.Equals(record.DateOfBirth.ToString(this.culture), values[i + 2], StringComparison.InvariantCultureIgnoreCase),
                                isNeedAdd,
                                i);
                            break;
                        case "GENDER":
                            if (!char.TryParse(values[i + 2], out char gender))
                            {
                                throw new ArgumentException("Incorrect gender", nameof(values));
                            }

                            isNeedAdd = CheckOperator(
                                values[i],
                                string.Equals(record.Gender.ToString(this.culture), gender.ToString(this.culture), StringComparison.InvariantCultureIgnoreCase),
                                isNeedAdd,
                                i);
                            break;
                        case "PASSPORTID":
                            if (!short.TryParse(values[i + 2], out short passportId))
                            {
                                throw new ArgumentException("Incorrect passportId", nameof(values));
                            }

                            isNeedAdd = CheckOperator(
                                values[i],
                                record.PassportId == passportId,
                                isNeedAdd,
                                i);
                            break;
                        case "SALARY":
                            if (!decimal.TryParse(values[i + 2], out decimal salary))
                            {
                                throw new ArgumentException("Incorrect salary", nameof(values));
                            }

                            isNeedAdd = CheckOperator(
                                values[i],
                                record.Salary == salary,
                                isNeedAdd,
                                i);
                            break;
                        default: throw new ArgumentException("Incorrect format", nameof(values));
                    }
                }

                if (isNeedAdd)
                {
                    resultList.Add(record.Id);
                }

                j++;
            }

            return resultList;
        }
    }
}
