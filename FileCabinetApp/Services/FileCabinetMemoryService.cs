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
            int id;
            if (recordData.Id > 0)
            {
                id = recordData.Id;
                this.Remove(id);
                this.presentIdList.Add(id, this.list.Count);
            }
            else
            {
                id = this.GetNextFreeId();
            }

            var record = new FileCabinetRecord
            {
                Id = id,
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

            List<FileCabinetRecord> foundResult = this.Find(values).ToList();
            List<int> deletedId = new List<int>();
            for (int i = 0; i < foundResult.Count; i++)
            {
                deletedId.Add(foundResult[i].Id);

                this.firstNameDictionary[foundResult[i].FirstName.ToUpper(this.englishUS)].Remove(foundResult[i]);
                this.lastNameDictionary[foundResult[i].LastName.ToUpper(this.englishUS)].Remove(foundResult[i]);
                this.dateOfBirthDictionary[foundResult[i].DateOfBirth.ToString(this.englishUS)].Remove(foundResult[i]);
                this.list.Remove(foundResult[i]);
            }

            return deletedId;
        }

        private static IEnumerable<FileCabinetRecord> FindById(IEnumerable<FileCabinetRecord> fileCabinetRecords, int id)
        {
            List<FileCabinetRecord> foundList = new List<FileCabinetRecord>();
            foreach (FileCabinetRecord record in fileCabinetRecords)
            {
                if (record.Id == id)
                {
                    foundList.Add(record);
                }
            }

            return foundList;
        }

        private static IEnumerable<FileCabinetRecord> FindByGender(IEnumerable<FileCabinetRecord> fileCabinetRecords, char gender)
        {
            List<FileCabinetRecord> foundList = new List<FileCabinetRecord>();
            foreach (FileCabinetRecord record in fileCabinetRecords)
            {
                if (record.Gender == gender)
                {
                    foundList.Add(record);
                }
            }

            return foundList;
        }

        private static IEnumerable<FileCabinetRecord> FindByPassportId(IEnumerable<FileCabinetRecord> fileCabinetRecords, short passportId)
        {
            List<FileCabinetRecord> foundList = new List<FileCabinetRecord>();
            foreach (FileCabinetRecord record in fileCabinetRecords)
            {
                if (record.PassportId == passportId)
                {
                    foundList.Add(record);
                }
            }

            return foundList;
        }

        private static IEnumerable<FileCabinetRecord> FindBySalary(IEnumerable<FileCabinetRecord> fileCabinetRecords, decimal salary)
        {
            List<FileCabinetRecord> foundList = new List<FileCabinetRecord>();
            foreach (FileCabinetRecord record in fileCabinetRecords)
            {
                if (record.Salary == salary)
                {
                    foundList.Add(record);
                }
            }

            return foundList;
        }

        private void AddToDictionary(Dictionary<string, List<FileCabinetRecord>> dictionary, string name, int id)
        {
            List<FileCabinetRecord> list;
            if (dictionary.ContainsKey(name.ToUpper(this.englishUS)))
            {
                dictionary[name.ToUpper(this.englishUS)].Add(this.list[this.presentIdList[id]]);
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
            while (i < list.Count && list[i].Id != id)
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

        private IEnumerable<FileCabinetRecord> Find(string[] values)
        {
            IEnumerable<FileCabinetRecord> modifiedList = this.list;
            for (int i = 0; i < values.Length; i += 3)
            {
                switch (values[i + 1].ToLower(this.englishUS))
                {
                    case "id":
                        if (!int.TryParse(values[i + 2], out int id))
                        {
                            throw new ArgumentException("Incorrect id", nameof(values));
                        }

                        if (values[i] == "and")
                        {
                            modifiedList = modifiedList.Intersect(FindById(modifiedList, id));
                        }
                        else
                        {
                            if (values[i] == "or")
                            {
                                modifiedList = modifiedList.Union(FindById(this.list, id));
                            }
                            else
                            {
                                if (i == 0 && values[i] == "where")
                                {
                                    modifiedList = FindById(this.list, id);
                                }
                                else
                                {
                                    throw new ArgumentException("Incorrect format", nameof(values));
                                }
                            }
                        }

                        break;
                    case "firstname":
                        if (values[i] == "and")
                        {
                            modifiedList = modifiedList.Intersect(this.firstNameDictionary[values[i + 2].ToUpper(this.englishUS)]);
                        }
                        else
                        {
                            if (values[i] == "or")
                            {
                                modifiedList = modifiedList.Union(this.firstNameDictionary[values[i + 2].ToUpper(this.englishUS)]);
                            }
                            else
                            {
                                if (i == 0 && values[i] == "where")
                                {
                                    modifiedList = this.firstNameDictionary[values[i + 2].ToUpper(this.englishUS)];
                                }
                                else
                                {
                                    throw new ArgumentException("Incorrect format", nameof(values));
                                }
                            }
                        }

                        break;
                    case "lastname":
                        if (values[i] == "and")
                        {
                            modifiedList = modifiedList.Intersect(this.lastNameDictionary[values[i + 2].ToUpper(this.englishUS)]);
                        }
                        else
                        {
                            if (values[i] == "or")
                            {
                                modifiedList = modifiedList.Union(this.lastNameDictionary[values[i + 2].ToUpper(this.englishUS)]);
                            }
                            else
                            {
                                if (i == 0 && values[i] == "where")
                                {
                                    modifiedList = this.lastNameDictionary[values[i + 2].ToUpper(this.englishUS)];
                                }
                                else
                                {
                                    throw new ArgumentException("Incorrect format", nameof(values));
                                }
                            }
                        }

                        break;
                    case "dateofbirth":
                        if (values[i] == "and")
                        {
                            modifiedList = modifiedList.Intersect(this.dateOfBirthDictionary[values[i + 2].ToUpper(this.englishUS)]);
                        }
                        else
                        {
                            if (values[i] == "or")
                            {
                                modifiedList = modifiedList.Union(this.dateOfBirthDictionary[values[i + 2].ToUpper(this.englishUS)]);
                            }
                            else
                            {
                                if (i == 0 && values[i] == "where")
                                {
                                    modifiedList = this.dateOfBirthDictionary[values[i + 2].ToUpper(this.englishUS)];
                                }
                                else
                                {
                                    throw new ArgumentException("Incorrect format", nameof(values));
                                }
                            }
                        }

                        break;
                    case "gender":
                        if (!char.TryParse(values[i + 2], out char gender))
                        {
                            throw new ArgumentException("Incorrect gender", nameof(values));
                        }

                        if (values[i] == "and")
                        {
                            modifiedList = modifiedList.Intersect(FindByGender(modifiedList, gender));
                        }
                        else
                        {
                            if (values[i] == "or")
                            {
                                modifiedList = modifiedList.Union(FindByGender(this.list, gender));
                            }
                            else
                            {
                                if (i == 0 && values[i] == "where")
                                {
                                    modifiedList = FindByGender(this.list, gender);
                                }
                                else
                                {
                                    throw new ArgumentException("Incorrect format", nameof(values));
                                }
                            }
                        }

                        break;
                    case "passportid":
                        if (!short.TryParse(values[i + 2], out short passportId))
                        {
                            throw new ArgumentException("Incorrect passportId", nameof(values));
                        }

                        if (values[i] == "and")
                        {
                            modifiedList = modifiedList.Intersect(FindByPassportId(modifiedList, passportId));
                        }
                        else
                        {
                            if (values[i] == "or")
                            {
                                modifiedList = modifiedList.Union(FindByPassportId(this.list, passportId));
                            }
                            else
                            {
                                if (i == 0 && values[i] == "where")
                                {
                                    modifiedList = FindByPassportId(this.list, passportId);
                                }
                                else
                                {
                                    throw new ArgumentException("Incorrect format", nameof(values));
                                }
                            }
                        }

                        break;
                    case "salary":
                        if (!decimal.TryParse(values[i + 2], out decimal salary))
                        {
                            throw new ArgumentException("Incorrect passportId", nameof(values));
                        }

                        if (values[i] == "and")
                        {
                            modifiedList = modifiedList.Intersect(FindBySalary(modifiedList, salary));
                        }
                        else
                        {
                            if (values[i] == "or")
                            {
                                modifiedList = modifiedList.Union(FindBySalary(this.list, salary));
                            }
                            else
                            {
                                if (i == 0 && values[i] == "where")
                                {
                                    modifiedList = FindBySalary(this.list, salary);
                                }
                                else
                                {
                                    throw new ArgumentException("Incorrect format", nameof(values));
                                }
                            }
                        }

                        break;
                    default:
                        throw new ArgumentException("Incorrect format", nameof(values));
                }
            }

            return modifiedList;
        }
    }
}
