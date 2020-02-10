using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for File Cabinet.
    /// </summary>
    public class FileCabinetService
    {
        /// <summary>
        /// Min salary for Belarus.
        /// </summary>
        public const int MinSalary = 375;
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// Create a new record.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <param name="lastName">User's last name.</param>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <param name="gender">User's gender.</param>
        /// <param name="passportId">User's passport id.</param>
        /// <param name="salary">User's salary.</param>
        /// <returns>Id of a new record.</returns>
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
            List<FileCabinetRecord> listByFirstName = new List<FileCabinetRecord>();
            this.AddToDictionary(this.firstNameDictionary, listByFirstName, firstName, record.Id);
            List<FileCabinetRecord> listByLastName = new List<FileCabinetRecord>();
            this.AddToDictionary(this.lastNameDictionary, listByLastName, lastName, record.Id);
            List<FileCabinetRecord> listBydateOfBirth = new List<FileCabinetRecord>();
            this.AddToDictionary(this.dateOfBirthDictionary, listBydateOfBirth, dateOfBirth.ToString(this.englishUS), record.Id);
            return record.Id;
        }

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>All records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        /// <summary>
        /// Returns count of records.
        /// </summary>
        /// <returns>Count of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Edit record by id.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <param name="firstName">User's first name.</param>
        /// <param name="lastName">User's last name.</param>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <param name="gender">User's gender.</param>
        /// <param name="passportId">User's passport id.</param>
        /// <param name="salary">User's salary.</param>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, char gender, short passportId, decimal salary)
        {
            if (id > this.list.Count)
            {
                throw new ArgumentException($"#{id} record is not found.", nameof(id));
            }

            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName), "Firstname can't be null");
            }

            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(firstName), "Lastname can't be null");
            }

            List<FileCabinetRecord> listByFirstName = this.firstNameDictionary[this.list[id - 1].FirstName.ToUpper(this.englishUS)];
            this.RemoveFromDictionary(this.firstNameDictionary, listByFirstName, lastName, id);
            List<FileCabinetRecord> listByLastName = this.lastNameDictionary[this.list[id - 1].LastName.ToUpper(this.englishUS)];
            this.RemoveFromDictionary(this.lastNameDictionary, listByLastName, lastName, id);
            List<FileCabinetRecord> listByDateOfBirth = this.dateOfBirthDictionary[this.list[id - 1].DateOfBirth.ToString(this.englishUS)];
            this.RemoveFromDictionary(this.dateOfBirthDictionary, listByDateOfBirth, dateOfBirth.ToString(this.englishUS), id);

            this.list[id - 1].FirstName = firstName;
            this.list[id - 1].LastName = lastName;
            this.list[id - 1].DateOfBirth = dateOfBirth;
            this.list[id - 1].Gender = gender;
            this.list[id - 1].PassportId = passportId;
            this.list[id - 1].Salary = salary;

            this.AddToDictionary(this.firstNameDictionary, listByFirstName, firstName, id);
            this.AddToDictionary(this.lastNameDictionary, listByLastName, lastName, id);
            this.AddToDictionary(this.dateOfBirthDictionary, listByDateOfBirth, dateOfBirth.ToString(this.englishUS), id);
        }

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <returns>Records whith sought-for firstName.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName), "Firstname can't be null");
            }

            firstName = firstName.ToUpper(this.englishUS);
            List<FileCabinetRecord> listByFirstName = new List<FileCabinetRecord>();
            if (this.firstNameDictionary.ContainsKey(firstName.ToUpper(this.englishUS)))
            {
                listByFirstName = this.firstNameDictionary[firstName.ToUpper(this.englishUS)];
            }

            return listByFirstName.ToArray();
        }

        /// <summary>
        /// Finds records by last name.
        /// </summary>
        /// <param name="lastName">User's last name.</param>
        /// <returns>Records whith sought-for lastName.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName), "Lastname can't be null");
            }

            lastName = lastName.ToUpper(this.englishUS);
            List<FileCabinetRecord> listByLastName = new List<FileCabinetRecord>();
            if (this.lastNameDictionary.ContainsKey(lastName.ToUpper(this.englishUS)))
            {
                listByLastName = this.lastNameDictionary[lastName.ToUpper(this.englishUS)];
            }

            return listByLastName.ToArray();
        }

        /// <summary>
        /// Finds records by DateOfBirth.
        /// </summary>
        /// <param name="date">User's date of Birth.</param>
        /// <returns>Records whith sought-for date of Birth.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(DateTime date)
        {
            List<FileCabinetRecord> listByDateOfBirth = new List<FileCabinetRecord>();
            if (this.dateOfBirthDictionary.ContainsKey(date.ToString(this.englishUS)))
            {
                listByDateOfBirth = this.dateOfBirthDictionary[date.ToString(this.englishUS)];
            }

            return listByDateOfBirth.ToArray();
        }

        private void AddToDictionary(Dictionary<string, List<FileCabinetRecord>> dictionary, List<FileCabinetRecord> list, string name, int id)
        {
            if (dictionary.ContainsKey(name))
            {
                list = dictionary[name];
            }
            else
            {
                list = new List<FileCabinetRecord>();
            }

            list.Add(this.list[id - 1]);
            dictionary.Add(name.ToUpper(this.englishUS), list);
        }

        private void RemoveFromDictionary(Dictionary<string, List<FileCabinetRecord>> dictionary, List<FileCabinetRecord> list, string name, int id)
        {
            int itemByFirstName = list.IndexOf(this.list[id - 1]);
            list.RemoveAt(itemByFirstName);
            if (list.Count > 0)
            {
                dictionary[name] = list;
            }
            else
            {
                dictionary.Remove(this.list[id - 1].FirstName.ToUpper(this.englishUS));
            }
        }
    }
}
