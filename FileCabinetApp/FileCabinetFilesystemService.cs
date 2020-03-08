using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetService with Filesystem.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
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

            this.fileStream.Write(this.enc.GetBytes("status"), 0, 2);
            this.fileStream.Write(BitConverter.GetBytes(this.GetStat() + 1), 0, 4);
            byte[] tempFirstName = this.enc.GetBytes(recordData.FirstName);
            Array.Resize(ref tempFirstName, 120);
            this.fileStream.Write(tempFirstName, 0, 120);
            byte[] tempLastName = this.enc.GetBytes(recordData.LastName);
            Array.Resize(ref tempLastName, 120);
            this.fileStream.Write(tempLastName, 0, 120);
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

            return this.GetStat();
        }

        /// <summary>
        /// Edit record by id.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <param name="recordData">User's data.</param>
        public void EditRecord(int id, RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            this.validator.ValidateParametrs(recordData);

            long i = 0;
            int currentId = -1;
            byte[] temp = new byte[4];
            while (currentId != id && i < this.GetStat())
            {
                this.fileStream.Seek((i * 278) + 2, SeekOrigin.Begin);

                this.fileStream.Read(temp, 0, 4);
                currentId = BitConverter.ToInt32(temp, 0);
                i++;
            }

            if (currentId == id)
            {
                byte[] tempFirstName = this.enc.GetBytes(recordData.FirstName);
                Array.Resize(ref tempFirstName, 120);
                this.fileStream.Write(tempFirstName, 0, 120);
                byte[] tempLastName = this.enc.GetBytes(recordData.LastName);
                Array.Resize(ref tempLastName, 120);
                this.fileStream.Write(tempLastName, 0, 120);
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
        }

        /// <summary>
        /// Finds records by DateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <returns>Records whith sought-for date of Birth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            List<FileCabinetRecord> listByDateOfBirth = new List<FileCabinetRecord>();
            for (int i = 0; i < this.GetStat(); i++)
            {
                this.fileStream.Seek((i * 278) + 246, SeekOrigin.Begin);
                byte[] temp = new byte[12];
                this.fileStream.Read(temp, 0, 4);
                int year = BitConverter.ToInt32(temp, 0);

                this.fileStream.Read(temp, 4, 4);
                int month = BitConverter.ToInt32(temp, 4);

                this.fileStream.Read(temp, 8, 4);
                int day = BitConverter.ToInt32(temp, 8);

                if (year == dateOfBirth.Year && month == dateOfBirth.Month && day == dateOfBirth.Day)
                {
                    this.fileStream.Seek(i * 278, SeekOrigin.Begin);
                    listByDateOfBirth.Add(this.ReadFromFile());
                    this.fileStream.Seek(i * 278, SeekOrigin.Begin);
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
                this.fileStream.Seek((i * 278) + 6, SeekOrigin.Begin);
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
                    this.fileStream.Seek(i * 278, SeekOrigin.Begin);
                    listByFirstName.Add(this.ReadFromFile());
                    this.fileStream.Seek(i * 278, SeekOrigin.Begin);
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
            long count = this.GetStat();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (long i = 0; i < count; i++)
            {
                FileCabinetRecord record = this.ReadFromFile();

                list.Add(record);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(list);
        }

        /// <summary>
        /// Returns count of records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public int GetStat()
        {
            return (int)(this.fileStream.Seek(0, SeekOrigin.End) / 278);
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
                this.fileStream.Seek((i * 278) + 126, SeekOrigin.Begin);
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
                    this.fileStream.Seek(i * 278, SeekOrigin.Begin);
                    listByLastName.Add(this.ReadFromFile());
                    this.fileStream.Seek(i * 278, SeekOrigin.Begin);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(listByLastName);
        }

        private FileCabinetRecord ReadFromFile()
        {
            byte[] temp = new byte[38];
            FileCabinetRecord record = new FileCabinetRecord();

            this.fileStream.Read(temp, 0, 2);
            short status = BitConverter.ToInt16(temp, 0);

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
    }
}
