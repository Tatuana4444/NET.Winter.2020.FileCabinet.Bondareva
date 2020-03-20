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
        private IRecordValidator validator;
        private Encoding enc = Encoding.Unicode;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        /// <param name="validator">Validator for params.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
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

            int id = this.GetNextFreeId();
            this.WriteToFile(recordData, id, this.offsetById[id]);

            return id;
        }

        /// <summary>
        /// Edit record by id.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <param name="recordData">User's data.</param>
        /// <exception cref="KeyNotFoundException">Throws when record this id doesn't exist.</exception>
        public void EditRecord(int id, RecordData recordData)
        {
            long pos = this.offsetById[id];

            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            this.validator.ValidateParametrs(recordData);

            this.WriteToFile(recordData, id, pos);
        }

        /// <summary>
        /// Finds records by DateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <returns>Records whith sought-for date of Birth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            List<FileCabinetRecord> listByDateOfBirth = new List<FileCabinetRecord>();
            for (int i = 0; i < this.fileStream.Length / 278; i++)
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
                    FileCabinetRecord record = this.ReadFromFile();
                    if (record != null)
                    {
                        listByDateOfBirth.Add(record);
                    }
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(listByDateOfBirth);
        }

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <returns>Records whith sought-for firstName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName), "Firstname can't be null");
            }

            firstName = firstName.ToUpper(this.englishUS);
            List<FileCabinetRecord> listByFirstName = new List<FileCabinetRecord>();
            for (int i = 0; i < this.GetStat(); i++)
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
                    FileCabinetRecord record = this.ReadFromFile();
                    if (record != null)
                    {
                        listByFirstName.Add(record);
                    }
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(listByFirstName);
        }

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>All records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            this.fileStream.Position = 0;
            for (long i = 0; i < this.GetStat(); i++)
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
        /// Returns count of records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public int GetStat()
        {
            return (int)(this.fileStream.Length / 278);
        }

        /// <summary>
        /// Finds records by last name.
        /// </summary>
        /// <param name="lastName">User's last name.</param>
        /// <returns>Records whith sought-for lastName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName), "Lastname can't be null");
            }

            lastName = lastName.ToUpper(this.englishUS);
            List<FileCabinetRecord> listByLastName = new List<FileCabinetRecord>();
            for (int i = 0; i < this.GetStat(); i++)
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
                    FileCabinetRecord record = this.ReadFromFile();
                    if (record != null)
                    {
                        listByLastName.Add(record);
                    }
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(listByLastName);
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
        /// Remove record by id.
        /// </summary>
        /// <param name="id">Id record.</param>
        /// <returns>True, if record exists, otherway returns false.</returns>
        public bool Remove(int id)
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
    }
}
