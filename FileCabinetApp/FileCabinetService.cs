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
        private IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">Validator for params.</param>
        public FileCabinetService(IRecordValidator validator)
        {
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

            this.validator.ValidateParametrs(recordData);
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = recordData.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Gender = recordData.Gender,
                PassportId = recordData.PassportId,
                Salary = recordData.Salary,
            };
            this.list.Add(record);
            List<FileCabinetRecord> listByFirstName = new List<FileCabinetRecord>();
            this.AddToDictionary(this.firstNameDictionary, listByFirstName, recordData.FirstName, record.Id);
            List<FileCabinetRecord> listByLastName = new List<FileCabinetRecord>();
            this.AddToDictionary(this.lastNameDictionary, listByLastName, recordData.LastName, record.Id);
            List<FileCabinetRecord> listBydateOfBirth = new List<FileCabinetRecord>();
            this.AddToDictionary(this.dateOfBirthDictionary, listBydateOfBirth, recordData.DateOfBirth.ToString(this.englishUS), record.Id);
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
        /// <param name="recordData">User's data.</param>
        public void EditRecord(int id, RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            this.validator.ValidateParametrs(recordData);

            List<FileCabinetRecord> listByFirstName = this.firstNameDictionary[this.list[id - 1].FirstName.ToUpper(this.englishUS)];
            this.RemoveFromDictionary(this.firstNameDictionary, listByFirstName, recordData.LastName, id);
            List<FileCabinetRecord> listByLastName = this.lastNameDictionary[this.list[id - 1].LastName.ToUpper(this.englishUS)];
            this.RemoveFromDictionary(this.lastNameDictionary, listByLastName, recordData.LastName, id);
            List<FileCabinetRecord> listByDateOfBirth = this.dateOfBirthDictionary[this.list[id - 1].DateOfBirth.ToString(this.englishUS)];
            this.RemoveFromDictionary(this.dateOfBirthDictionary, listByDateOfBirth, recordData.DateOfBirth.ToString(this.englishUS), id);

            this.list[id - 1].FirstName = recordData.FirstName;
            this.list[id - 1].LastName = recordData.LastName;
            this.list[id - 1].DateOfBirth = recordData.DateOfBirth;
            this.list[id - 1].Gender = recordData.Gender;
            this.list[id - 1].PassportId = recordData.PassportId;
            this.list[id - 1].Salary = recordData.Salary;

            this.AddToDictionary(this.firstNameDictionary, listByFirstName, recordData.FirstName, id);
            this.AddToDictionary(this.lastNameDictionary, listByLastName, recordData.LastName, id);
            this.AddToDictionary(this.dateOfBirthDictionary, listByDateOfBirth, recordData.DateOfBirth.ToString(this.englishUS), id);
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
