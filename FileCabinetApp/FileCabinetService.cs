using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        public const int MinSalary = 375;
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, char gender, short passportId, decimal salary)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName), "First name can't be null");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("Length of first name can't be less than 2 and more than 60", nameof(firstName));
            }

            if (firstName.Trim().Length == 0)
            {
                throw new ArgumentException("First name can't contain only spaces", nameof(firstName));
            }

            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName), "Last name can't be null");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Length of last name can't be less than 2 and more than 60", nameof(firstName));
            }

            if (lastName.Trim().Length == 0)
            {
                throw new ArgumentException("Last name can't contain only spaces", nameof(firstName));
            }

            if ((DateTime.Compare(new DateTime(1950, 1, 1), dateOfBirth) > 0)
                || (DateTime.Compare(DateTime.Now, dateOfBirth) < 0))
            {
                throw new ArgumentException("Date of Birth can't be less than 01-Jan-1950 and more than today", nameof(dateOfBirth));
            }

            if (gender != 'W' && gender != 'M')
            {
                throw new ArgumentException("Gender should be W or M", nameof(gender));
            }

            if (passportId < 1000 || passportId > 9999)
            {
                throw new ArgumentException("Passport Id can't be less than 1000 and more than 9999", nameof(passportId));
            }

            if (salary < MinSalary)
            {
                throw new ArgumentException($"Salary can't be less than {MinSalary}", nameof(salary));
            }

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                PassportId = passportId,
                Salary = salary,
            };

            this.list.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, char gender, short passportId, decimal salary)
        {
            if (id > this.list.Count)
            {
                throw new ArgumentException($"#{id} record is not found.", nameof(id));
            }

            this.list[id - 1].FirstName = firstName;
            this.list[id - 1].LastName = lastName;
            this.list[id - 1].DateOfBirth = dateOfBirth;
            this.list[id - 1].Gender = gender;
            this.list[id - 1].PassportId = passportId;
            this.list[id - 1].Salary = salary;
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName), "Firstname can't be null");
            }

            CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");
            firstName = firstName.ToUpper(englishUS);
            List<FileCabinetRecord> listByFirstName = new List<FileCabinetRecord>();
            foreach (FileCabinetRecord fileCabinetRecord in this.list)
            {
                if (fileCabinetRecord.FirstName.ToUpper(englishUS) == firstName)
                {
                    listByFirstName.Add(fileCabinetRecord);
                }
            }

            return listByFirstName.ToArray();
        }
    }
}
