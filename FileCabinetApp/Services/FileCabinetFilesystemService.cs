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
        private readonly Dictionary<int, long> offsetById = new Dictionary<int, long>();

        private readonly CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");
        private FileStream fileStream;
        private IValidator validator;
        private Encoding enc = Encoding.Unicode;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        /// <param name="validator">Validator for params.</param>
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
        public int CreateRecord(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

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
        /// Finds records by DateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <returns>Records whith sought-for date of Birth.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            for (int i = 0; i < this.fileStream.Length / 278; i++)
            {
                FileCabinetRecord result = GetRecordByDate(i);
                if (result != null)
                {
                    yield return result;
                }
            }

            FileCabinetRecord GetRecordByDate(int i)
            {
                this.fileStream.Position = (i * 278) + 246;
                byte[] temp = new byte[12];
                this.fileStream.Read(temp, 0, 4);
                int year = BitConverter.ToInt32(temp, 0);

                this.fileStream.Read(temp, 4, 4);
                int month = BitConverter.ToInt32(temp, 4);

                this.fileStream.Read(temp, 8, 4);
                int day = BitConverter.ToInt32(temp, 8);

                if (year == dateOfBirth.Year && month == dateOfBirth.Month && day == dateOfBirth.Day)
                {
                    this.fileStream.Position = i * 278;
                    return this.ReadFromFile();
                }

                return null;
            }
        }

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <returns>Records whith sought-for firstName.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName), "Firstname can't be null");
            }

            firstName = firstName.ToUpper(this.englishUS);

            for (int i = 0; i < this.fileStream.Length / 278; i++)
            {
                FileCabinetRecord result = GetRecordFirstName(i);
                if (result != null)
                {
                    yield return result;
                }
            }

            FileCabinetRecord GetRecordFirstName(int i)
            {
                this.fileStream.Position = (i * 278) + 6;
                byte[] tempForStrings = new byte[120];
                this.fileStream.Read(tempForStrings, 0, 120);
                int end = 0;
                while (tempForStrings[end] != '\0' || tempForStrings[end + 1] != '\0')
                {
                    end += 2;
                }

                Array.Resize(ref tempForStrings, end);
                string currentFirstName = this.enc.GetString(tempForStrings);

                if (currentFirstName.ToUpper(this.englishUS) == firstName)
                {
                    this.fileStream.Position = i * 278;
                    return this.ReadFromFile();
                }

                return null;
            }
        }

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>All records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            this.fileStream.Position = 0;
            for (long i = 0; i < this.GetStat().Item1; i++)
            {
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
            while (i < this.fileStream.Length / 278)
            {
                this.fileStream.Position = (i * 278) + 1;
                int b = this.fileStream.ReadByte();
                if ((b & 2) == 2)
                {
                    deletedCount++;
                }

                i++;
            }

            this.fileStream.Position = pos;
            return new Tuple<int, int>((int)(this.fileStream.Length / 278), deletedCount);
        }

        /// <summary>
        /// Finds records by last name.
        /// </summary>
        /// <param name="lastName">User's last name.</param>
        /// <returns>Records whith sought-for lastName.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName), "Lastname can't be null");
            }

            lastName = lastName.ToUpper(this.englishUS);

            for (int i = 0; i < this.fileStream.Length / 278; i++)
            {
                FileCabinetRecord result = GetRecordLastName(i);
                if (result != null)
                {
                    yield return result;
                }
            }

            FileCabinetRecord GetRecordLastName(int i)
            {
                this.fileStream.Position = (i * 278) + 126;
                byte[] tempForStrings = new byte[120];
                this.fileStream.Read(tempForStrings, 0, 120);
                int end = 0;
                while (tempForStrings[end] != '\0' || tempForStrings[end + 1] != '\0')
                {
                    end += 2;
                }

                Array.Resize(ref tempForStrings, end);
                string currentFirstName = this.enc.GetString(tempForStrings);

                if (currentFirstName.ToUpper(this.englishUS) == lastName)
                {
                    this.fileStream.Position = i * 278;
                    return this.ReadFromFile();
                }

                return null;
            }
        }

        /// <summary>
        /// Restore date from snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot.</param>
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
            while (i < this.fileStream.Length / 278)
            {
                this.fileStream.Position = (i * 278) + 1;
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

            this.fileStream.SetLength(this.fileStream.Length - (offsetCount * 278));

            return purgedCount + offsetCount;
        }

        /// <summary>
        /// Delete record by parameters.
        /// </summary>
        /// <param name="param">Record parameters.</param>
        /// <returns>List of id recored, that was deleted.</returns>
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
            }

            return foundResult;
        }

        /// <summary>
        /// Update records by parameters.
        /// </summary>
        /// <param name="param">Record parameters.</param>
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

            string whereParams = param.Substring(whereIndex, param.Length - whereIndex);
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

            for (int j = 0; j < foundResult.Count; j++)
            {
                for (int i = 0; i < setValues.Length; i += 2)
                {
                    RecordData recordData;
                    switch (setValues[i].ToLower(this.englishUS))
                    {
                        case "firstname":
                            recordData = new RecordData() { FirstName = setValues[i + 1] };
                            this.validator.ValidatePrameter("firstname", recordData);
                            this.WriteFirstName(setValues[i + 1], this.offsetById[foundResult[j]]);
                            break;
                        case "lastname":
                            recordData = new RecordData() { LastName = setValues[i + 1] };
                            this.validator.ValidatePrameter("lastname", recordData);
                            this.WriteLastName(setValues[i + 1], this.offsetById[foundResult[j]]);
                            break;
                        case "dateofbirth":
                            if (!DateTime.TryParse(setValues[i + 1], this.englishUS, DateTimeStyles.None, out DateTime dateOfBirth))
                            {
                                throw new ArgumentException("Incorrect dateofbith", nameof(param));
                            }

                            recordData = new RecordData() { DateOfBirth = dateOfBirth };
                            this.validator.ValidatePrameter("dateofbirth", recordData);
                            this.WriteDateOfBirth(dateOfBirth, this.offsetById[foundResult[j]]);
                            break;
                        case "gender":
                            if (!char.TryParse(setValues[i + 1], out char gender))
                            {
                                throw new ArgumentException("Incorrect gender", nameof(param));
                            }

                            recordData = new RecordData() { Gender = gender };
                            this.validator.ValidatePrameter("gender", recordData);
                            this.WriteGender(gender, this.offsetById[foundResult[j]]);
                            break;
                        case "passportid":
                            if (!short.TryParse(setValues[i + 1], out short passportId))
                            {
                                throw new ArgumentException("Incorrect passportId", nameof(param));
                            }

                            recordData = new RecordData() { PassportId = passportId };
                            this.validator.ValidatePrameter("passportid", recordData);
                            this.WritePassportId(passportId, this.offsetById[foundResult[j]]);
                            break;
                        case "salary":
                            if (!decimal.TryParse(setValues[i + 1], out decimal salary))
                            {
                                throw new ArgumentException("Incorrect salary", nameof(param));
                            }

                            recordData = new RecordData() { Salary = salary };
                            this.validator.ValidatePrameter("salary", recordData);
                            this.WriteSalary(salary, this.offsetById[foundResult[j]]);
                            break;
                        default: throw new ArgumentException("Incorrect format", nameof(param));
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
            while (i < this.fileStream.Length / 278)
            {
                this.fileStream.Position = i * 278;
                FileCabinetRecord record = this.ReadFromFile();
                RecordData recordData = new RecordData(record.FirstName, record.LastName, record.DateOfBirth, record.Gender, record.PassportId, record.Salary);
                this.WriteToFile(recordData, record.Id, (i - offsetCount) * 278);
                i++;
            }

            this.fileStream.SetLength(this.fileStream.Length - (offsetCount * 278));
        }

        private void WriteFirstName(string firstName, long pos)
        {
            this.fileStream.Position = pos + 6;
            byte[] tempFirstName = this.enc.GetBytes(firstName);
            Array.Resize(ref tempFirstName, 120);
            this.fileStream.Write(tempFirstName, 0, 120);
        }

        private void WriteLastName(string lastName, long pos)
        {
            this.fileStream.Position = pos + 126;
            byte[] tempLastName = this.enc.GetBytes(lastName);
            Array.Resize(ref tempLastName, 120);
            this.fileStream.Write(tempLastName, 0, 120);
        }

        private void WriteDateOfBirth(DateTime dateOfBirth, long pos)
        {
            this.fileStream.Position = pos + 246;
            this.fileStream.Write(BitConverter.GetBytes(dateOfBirth.Year), 0, 4);
            this.fileStream.Write(BitConverter.GetBytes(dateOfBirth.Month), 0, 4);
            this.fileStream.Write(BitConverter.GetBytes(dateOfBirth.Day), 0, 4);
        }

        private void WriteGender(char gender, long pos)
        {
            this.fileStream.Position = pos + 258;
            this.fileStream.Write(BitConverter.GetBytes(gender), 0, 2);
        }

        private void WritePassportId(short passportId, long pos)
        {
            this.fileStream.Position = pos + 260;
            this.fileStream.Write(BitConverter.GetBytes(passportId), 0, 2);
        }

        private void WriteSalary(decimal salary, long pos)
        {
            this.fileStream.Position = pos + 262;
            int[] sal = decimal.GetBits(salary);
            foreach (var s in sal)
            {
                this.fileStream.Write(BitConverter.GetBytes(s), 0, 4);
            }
        }

        private void WriteToFile(RecordData recordData,  int id, long pos)
        {
            this.fileStream.Position = pos;

            this.fileStream.Write(BitConverter.GetBytes(0), 0, 2);
            this.fileStream.Write(BitConverter.GetBytes(id), 0, 4);
            byte[] tempFirstName = this.enc.GetBytes(recordData.FirstName);
            this.fileStream.Write(tempFirstName, 0, tempFirstName.Length);
            this.fileStream.Position += 120 - tempFirstName.Length;
            byte[] tempLastName = this.enc.GetBytes(recordData.LastName);
            this.fileStream.Write(tempLastName, 0, tempLastName.Length);
            this.fileStream.Position += 120 - tempLastName.Length;
            this.fileStream.Write(BitConverter.GetBytes(recordData.DateOfBirth.Year), 0, 4);
            this.fileStream.Write(BitConverter.GetBytes(recordData.DateOfBirth.Month), 0, 4);
            this.fileStream.Write(BitConverter.GetBytes(recordData.DateOfBirth.Day), 0, 4);
            this.fileStream.Write(BitConverter.GetBytes(recordData.Gender), 0, 2);
            this.fileStream.Write(BitConverter.GetBytes(recordData.PassportId), 0, 2);
            int[] sal = decimal.GetBits(recordData.Salary);
            foreach (var s in sal)
            {
                this.fileStream.Write(BitConverter.GetBytes(s), 0, 4);
            }

            this.fileStream.Flush();
        }

        private FileCabinetRecord ReadFromFile()
        {
            byte[] temp = new byte[38];
            FileCabinetRecord record = new FileCabinetRecord();

            this.fileStream.Read(temp, 0, 2);
            if ((temp[1] & 2) == 2)
            {
                this.fileStream.Position += 276;
                return null;
            }

            this.fileStream.Read(temp, 2, 4);
            record.Id = BitConverter.ToInt32(temp, 2);

            byte[] tempForStrings = new byte[120];
            this.fileStream.Read(tempForStrings, 0, 120);
            int end = 0;
            while (tempForStrings[end] != '\0' || tempForStrings[end + 1] != '\0')
            {
                end += 2;
            }

            Array.Resize(ref tempForStrings, end);
            record.FirstName = this.enc.GetString(tempForStrings);

            tempForStrings = new byte[120];
            this.fileStream.Read(tempForStrings, 0, 120);
            end = 0;
            while (tempForStrings[end] != '\0' || tempForStrings[end + 1] != '\0')
            {
                end += 2;
            }

            Array.Resize(ref tempForStrings, end);
            record.LastName = this.enc.GetString(tempForStrings);

            this.fileStream.Read(temp, 6, 4);
            int year = BitConverter.ToInt32(temp, 6);

            this.fileStream.Read(temp, 10, 4);
            int month = BitConverter.ToInt32(temp, 10);

            this.fileStream.Read(temp, 14, 4);
            int day = BitConverter.ToInt32(temp, 14);

            record.DateOfBirth = new DateTime(year, month, day);

            this.fileStream.Read(temp, 18, 2);
            record.Gender = BitConverter.ToChar(temp, 18);

            this.fileStream.Read(temp, 20, 2);
            record.PassportId = BitConverter.ToInt16(temp, 20);

            int[] bytes = new int[4];
            for (int j = 0; j < 4; j++)
            {
                this.fileStream.Read(temp, 22 + (j * 4), 4);
                bytes[j] = BitConverter.ToInt32(temp, 22 + (j * 4));
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
            while (j < this.fileStream.Length / 278)
            {
                FileCabinetRecord record = this.ReadFromFile();
                bool isNeedAdd = true;
                for (int i = 0; i < values.Length; i += 3)
                {
                    switch (values[i + 1].ToLower(this.englishUS))
                    {
                        case "id":
                            if (!int.TryParse(values[i + 2], out int id))
                            {
                                throw new ArgumentException("Incorrect id", nameof(values));
                            }

                            if (record.Id == id)
                            {
                                if (values[i] == "or")
                                {
                                    isNeedAdd = true;
                                }
                                else
                                {
                                    if (values[i] != "and" && !(i == 0 && values[i] == "where"))
                                    {
                                        throw new ArgumentException("Incorrect format", nameof(values));
                                    }
                                }
                            }
                            else
                            {
                                if (!(isNeedAdd && values[i] == "or"))
                                {
                                    isNeedAdd = false;
                                }
                            }

                            break;
                        case "firstname":
                            if (record.FirstName.ToLower(this.englishUS) == values[i + 2].ToLower(this.englishUS))
                            {
                                if (values[i] == "or")
                                {
                                    isNeedAdd = true;
                                }
                                else
                                {
                                    if (values[i] != "and" && !(i == 0 && values[i] == "where"))
                                    {
                                        throw new ArgumentException("Incorrect format", nameof(values));
                                    }
                                }
                            }
                            else
                            {
                                if (!(isNeedAdd && values[i] == "or"))
                                {
                                    isNeedAdd = false;
                                }
                            }

                            break;
                        case "lastname":
                            if (record.LastName.ToLower(this.englishUS) == values[i + 2].ToLower(this.englishUS))
                            {
                                if (values[i] == "or")
                                {
                                    isNeedAdd = true;
                                }
                                else
                                {
                                    if (values[i] != "and" && !(i == 0 && values[i] == "where"))
                                    {
                                        throw new ArgumentException("Incorrect format", nameof(values));
                                    }
                                }
                            }
                            else
                            {
                                if (!(isNeedAdd && values[i] == "or"))
                                {
                                    isNeedAdd = false;
                                }
                            }

                            break;
                        case "dateofbirth":
                            if (record.DateOfBirth.ToString(this.englishUS) == values[i + 2])
                            {
                                if (values[i] == "or")
                                {
                                    isNeedAdd = true;
                                }
                                else
                                {
                                    if (values[i] != "and" && !(i == 0 && values[i] == "where"))
                                    {
                                        throw new ArgumentException("Incorrect format", nameof(values));
                                    }
                                }
                            }
                            else
                            {
                                if (!(isNeedAdd && values[i] == "or"))
                                {
                                    isNeedAdd = false;
                                }
                            }

                            break;
                        case "gender":
                            if (!char.TryParse(values[i + 2], out char gender))
                            {
                                throw new ArgumentException("Incorrect gender", nameof(values));
                            }

                            if (record.Gender == gender)
                            {
                                if (values[i] == "or")
                                {
                                    isNeedAdd = true;
                                }
                                else
                                {
                                    if (values[i] != "and" && !(i == 0 && values[i] == "where"))
                                    {
                                        throw new ArgumentException("Incorrect format", nameof(values));
                                    }
                                }
                            }
                            else
                            {
                                if (!(isNeedAdd && values[i] == "or"))
                                {
                                    isNeedAdd = false;
                                }
                            }

                            break;
                        case "passportid":
                            if (!short.TryParse(values[i + 2], out short passportId))
                            {
                                throw new ArgumentException("Incorrect passportId", nameof(values));
                            }

                            if (record.PassportId == passportId)
                            {
                                if (values[i] == "or")
                                {
                                    isNeedAdd = true;
                                }
                                else
                                {
                                    if (values[i] != "and" && !(i == 0 && values[i] == "where"))
                                    {
                                        throw new ArgumentException("Incorrect format", nameof(values));
                                    }
                                }
                            }
                            else
                            {
                                if (!(isNeedAdd && values[i] == "or"))
                                {
                                    isNeedAdd = false;
                                }
                            }

                            break;
                        case "salary":
                            if (!decimal.TryParse(values[i + 2], out decimal salary))
                            {
                                throw new ArgumentException("Incorrect salary", nameof(values));
                            }

                            if (record.Salary == salary)
                            {
                                if (values[i] == "or")
                                {
                                    isNeedAdd = true;
                                }
                                else
                                {
                                    if (values[i] != "and" && !(i == 0 && values[i] == "where"))
                                    {
                                        throw new ArgumentException("Incorrect format", nameof(values));
                                    }
                                }
                            }
                            else
                            {
                                if (!(isNeedAdd && values[i] == "or"))
                                {
                                    isNeedAdd = false;
                                }
                            }

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
