using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for File Cabinet.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        /// <summary>
        /// Min salary for Belarus.
        /// </summary>
        public const int MinSalary = 375;
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private IRecordValidator validator;
        private Dictionary<int, int> presentIdList = new Dictionary<int, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Validator for params.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Create new snapshot.
        /// </summary>
        /// <returns>Snapshot.</returns>
        public static FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot();
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
                Id = this.GetNextFreeId(),
                FirstName = recordData.FirstName,
                LastName = recordData.LastName,
                DateOfBirth = recordData.DateOfBirth,
                Gender = recordData.Gender,
                PassportId = recordData.PassportId,
                Salary = recordData.Salary,
            };
            this.list.Add(record);
            this.AddToDictionary(this.firstNameDictionary, recordData.FirstName.ToUpper(this.englishUS), record.Id);
            this.AddToDictionary(this.lastNameDictionary, recordData.LastName.ToUpper(this.englishUS), record.Id);
            this.AddToDictionary(this.dateOfBirthDictionary, recordData.DateOfBirth.ToString(this.englishUS), record.Id);
            return record.Id;
        }

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>All records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Returns count of records and count of deleted records.
        /// </summary>
        /// <returns>Count of records  and 0.</returns>
        public Tuple<int, int> GetStat()
        {
            return new Tuple<int, int>(this.list.Count, 0);
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

            this.RemoveFromDictionary(this.firstNameDictionary, recordData.LastName, id);
            this.RemoveFromDictionary(this.lastNameDictionary, recordData.LastName, id);
            this.RemoveFromDictionary(this.dateOfBirthDictionary, recordData.DateOfBirth.ToString(this.englishUS), id);

            this.list[id - 1].FirstName = recordData.FirstName;
            this.list[id - 1].LastName = recordData.LastName;
            this.list[id - 1].DateOfBirth = recordData.DateOfBirth;
            this.list[id - 1].Gender = recordData.Gender;
            this.list[id - 1].PassportId = recordData.PassportId;
            this.list[id - 1].Salary = recordData.Salary;

            this.AddToDictionary(this.firstNameDictionary, recordData.FirstName.ToUpper(this.englishUS), id);
            this.AddToDictionary(this.lastNameDictionary, recordData.LastName.ToUpper(this.englishUS), id);
            this.AddToDictionary(this.dateOfBirthDictionary, recordData.DateOfBirth.ToString(this.englishUS), id);
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
            List<FileCabinetRecord> listByFirstName = new List<FileCabinetRecord>();
            if (this.firstNameDictionary.ContainsKey(firstName.ToUpper(this.englishUS)))
            {
                listByFirstName = this.firstNameDictionary[firstName.ToUpper(this.englishUS)];
            }

            return listByFirstName;
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
            List<FileCabinetRecord> listByLastName = new List<FileCabinetRecord>();
            if (this.lastNameDictionary.ContainsKey(lastName.ToUpper(this.englishUS)))
            {
                listByLastName = this.lastNameDictionary[lastName.ToUpper(this.englishUS)];
            }

            return listByLastName;
        }

        /// <summary>
        /// Finds records by DateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <returns>Records whith sought-for date of Birth.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            List<FileCabinetRecord> listByDateOfBirth = new List<FileCabinetRecord>();
            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth.ToString(this.englishUS)))
            {
                listByDateOfBirth = this.dateOfBirthDictionary[dateOfBirth.ToString(this.englishUS)];
            }

            return listByDateOfBirth;
        }

        /// <summary>
        /// Restore data fron snapshot.
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
                if (this.presentIdList.TryGetValue(record.Id, out int pos))
                {
                    this.RemoveFromDictionary(this.firstNameDictionary, this.list[pos].FirstName.ToUpper(this.englishUS), record.Id);
                    this.RemoveFromDictionary(this.lastNameDictionary, this.list[pos].LastName.ToUpper(this.englishUS), record.Id);
                    this.RemoveFromDictionary(this.dateOfBirthDictionary, this.list[pos].DateOfBirth.ToString(this.englishUS), record.Id);
                    this.list[pos] = record;
                }
                else
                {
                    this.list.Add(record);
                }

                this.AddToDictionary(this.firstNameDictionary, record.FirstName.ToUpper(this.englishUS), record.Id);
                this.AddToDictionary(this.lastNameDictionary, record.LastName.ToUpper(this.englishUS), record.Id);
                this.AddToDictionary(this.dateOfBirthDictionary, record.DateOfBirth.ToString(this.englishUS), record.Id);
            }
        }

        /// <summary>
        /// Remove record by id.
        /// </summary>
        /// <param name="id">Id record.</param>
        /// <returns>True, if record exists, otherway returns false.</returns>
        public bool Remove(int id)
        {
            int i = this.FindRecord(this.list, id);
            if (i == this.list.Count)
            {
                return false;
            }

            FileCabinetRecord record = this.list[i];
            this.list.RemoveAt(i);

            this.RemoveFromDictionary(this.firstNameDictionary, record.FirstName.ToUpper(this.englishUS), id);
            this.RemoveFromDictionary(this.lastNameDictionary, record.LastName.ToUpper(this.englishUS), id);
            this.RemoveFromDictionary(this.dateOfBirthDictionary, record.DateOfBirth.ToString(this.englishUS), id);
            this.presentIdList.Remove(id);

            return true;
        }

        /// <summary>
        /// For memory service does nothing.
        /// </summary>
        /// <returns>Returns 0.</returns>
        public int Purge()
        {
            return 0;
        }

        private void AddToDictionary(Dictionary<string, List<FileCabinetRecord>> dictionary, string name, int id)
        {
            List<FileCabinetRecord> list;
            if (dictionary.ContainsKey(name.ToUpper(this.englishUS)))
            {
                dictionary[name.ToUpper(this.englishUS)].Add(this.list[id - 1]);
            }
            else
            {
                list = new List<FileCabinetRecord>();
                list.Add(this.list[this.presentIdList[id]]);
                dictionary.Add(name.ToUpper(this.englishUS), list);
            }
        }

        private void RemoveFromDictionary(Dictionary<string, List<FileCabinetRecord>> dictionary, string name, int id)
        {
            List<FileCabinetRecord> list = dictionary[name];
            int i = 0;
            while (id != list[i].Id)
            {
                i++;
            }

            list.RemoveAt(i);
            if (list.Count > 0)
            {
                dictionary[name] = list;
            }
            else
            {
                dictionary.Remove(name);
            }
        }

        private int FindRecord(List<FileCabinetRecord> list, int id)
        {
            int i = 0;
            while (list[i].Id != id && i < list.Count)
            {
                i++;
            }

            return i;
        }

        private int GetNextFreeId()
        {
            int i = 1;
            while (i < this.presentIdList.Count)
            {
                if (!this.presentIdList.TryGetValue(i, out int _))
                {
                    return i;
                }

                i++;
            }

            this.presentIdList.Add(i, this.list.Count);
            return i;
        }
    }
}
