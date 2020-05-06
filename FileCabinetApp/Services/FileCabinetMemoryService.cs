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
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> cache = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly IValidator validator;
        private readonly Dictionary<int, int> presentIdList = new Dictionary<int, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Validator for params.</param>
        public FileCabinetMemoryService(IValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Create new snapshot.
        /// </summary>
        /// <returns>Snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.ToArray());
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
            this.AddToDictionary(this.firstNameDictionary, recordData.FirstName.ToUpperInvariant(), record.Id);
            this.AddToDictionary(this.lastNameDictionary, recordData.LastName.ToUpperInvariant(), record.Id);
            this.AddToDictionary(this.dateOfBirthDictionary, recordData.DateOfBirth.ToString(this.culture), record.Id);
            this.cache.Clear();
            return record.Id;
        }

        /// <summary>
        /// Returns records.
        /// </summary>
        /// <param name="filter">Record's filter. Filter start from 'where' and can contain 'and' and 'or'.</param>
        /// <returns>Records by filret.</returns>
        /// <exception cref="ArgumentNullException">Throw when param is null.</exception>
        /// <exception cref="ArgumentException">Throw when param not contains 'where', 'or', 'and' or have incorrect data.</exception>
        public ReadOnlyCollection<FileCabinetRecord> SelectRecords(string filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            List<FileCabinetRecord> foundResult;
            string filterString = string.Empty;
            if (filter.Length != 0)
            {
                string[] values = filter.Split(new string[] { " = '", " ='", "= '", "='", "' ", " " }, StringSplitOptions.RemoveEmptyEntries);
                values[^1] = values[^1][0..^1];
                if (values.Length % 3 != 0)
                {
                    throw new ArgumentException("Incorrect format", nameof(filter));
                }

                filterString = this.GetFilterString(values);
                foundResult = this.Find(values).ToList();
            }
            else
            {
                foundResult = this.list;
            }

            var filterStr = filterString.ToUpperInvariant();
            if (this.cache.ContainsKey(filterStr))
            {
                foundResult = this.cache[filterStr];
            }
            else
            {
                this.cache.Add(filterStr, foundResult);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(foundResult);
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
        /// Restore data fron snapshot.
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
                if (this.presentIdList.TryGetValue(record.Id, out int pos))
                {
                    RemoveFromDictionary(this.firstNameDictionary, this.list[pos].FirstName.ToUpperInvariant(), record.Id);
                    RemoveFromDictionary(this.lastNameDictionary, this.list[pos].LastName.ToUpperInvariant(), record.Id);
                    RemoveFromDictionary(this.dateOfBirthDictionary, this.list[pos].DateOfBirth.ToString(this.culture), record.Id);
                    this.list[pos] = record;
                }
                else
                {
                    this.list.Add(record);
                    this.presentIdList.Add(record.Id, this.list.Count - 1);
                }

                this.AddToDictionary(this.firstNameDictionary, record.FirstName.ToUpperInvariant(), record.Id);
                this.AddToDictionary(this.lastNameDictionary, record.LastName.ToUpperInvariant(), record.Id);
                this.AddToDictionary(this.dateOfBirthDictionary, record.DateOfBirth.ToString(this.culture), record.Id);
            }
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
        /// <exception cref="ArgumentNullException">Throw when param is null.</exception>
        /// <exception cref="ArgumentException">Throw when param not contains 'where', 'or', 'and' or have incorrect data.</exception>
        public IEnumerable<int> Delete(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentNullException(nameof(param), "Incorrect format");
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
                this.presentIdList.Remove(foundResult[i].Id);
                this.firstNameDictionary[foundResult[i].FirstName.ToUpperInvariant()].Remove(foundResult[i]);
                this.lastNameDictionary[foundResult[i].LastName.ToUpperInvariant()].Remove(foundResult[i]);
                this.dateOfBirthDictionary[foundResult[i].DateOfBirth.ToString(this.culture)].Remove(foundResult[i]);
                this.list.Remove(foundResult[i]);
            }

            this.cache.Clear();
            return deletedId;
        }

        /// <summary>
        /// Update records by parameters.
        /// </summary>
        /// <param name="param">Record parameters.</param>
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

            List<FileCabinetRecord> foundResult = this.Find(whereValues).ToList();

            string setParams = param.Substring(4, whereIndex - 5);
            string[] setValues = setParams.Split(new string[] { " = '", " ='", "= '", "='", "' , ", "', ", "'" }, StringSplitOptions.RemoveEmptyEntries);
            if (setValues.Length % 2 != 0)
            {
                throw new ArgumentException("Incorrect format", nameof(param));
            }

            this.UpdateRecords(foundResult, setValues);
            this.cache.Clear();
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
                if (string.Equals(record.Gender.ToString(CultureInfo.InvariantCulture), gender.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase))
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

        private static void RemoveFromDictionary(Dictionary<string, List<FileCabinetRecord>> dictionary, string name, int id)
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

        private static int FindRecord(List<FileCabinetRecord> list, int id)
        {
            int i = 0;
            while (i < list.Count && list[i].Id != id)
            {
                i++;
            }

            return i;
        }

        private void UpdateRecords(List<FileCabinetRecord> foundResult, string[] setValues)
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
                            foundResult[j].FirstName = setValues[i + 1];
                            break;
                        case "LASTNAME":
                            recordData = new RecordData() { LastName = setValues[i + 1] };
                            this.validator.ValidatePrameter("lastname", recordData);
                            foundResult[j].LastName = setValues[i + 1];
                            break;
                        case "DATEOFBIRTH":
                            if (!DateTime.TryParse(setValues[i + 1], this.culture, DateTimeStyles.None, out DateTime dateOfBirth))
                            {
                                throw new ArgumentException("Incorrect dateofbith", nameof(setValues));
                            }

                            recordData = new RecordData() { DateOfBirth = dateOfBirth };
                            this.validator.ValidatePrameter("dateofbirth", recordData);
                            foundResult[j].DateOfBirth = dateOfBirth;
                            break;
                        case "GENDER":
                            if (!char.TryParse(setValues[i + 1], out char gender))
                            {
                                throw new ArgumentException("Incorrect gender", nameof(setValues));
                            }

                            recordData = new RecordData() { Gender = gender };
                            this.validator.ValidatePrameter("gender", recordData);
                            foundResult[j].Gender = gender;
                            break;
                        case "PASSPORTID":
                            if (!short.TryParse(setValues[i + 1], out short passportId))
                            {
                                throw new ArgumentException("Incorrect passportId", nameof(setValues));
                            }

                            recordData = new RecordData() { PassportId = passportId };
                            this.validator.ValidatePrameter("passportid", recordData);
                            foundResult[j].PassportId = passportId;
                            break;
                        case "SALARY":
                            if (!decimal.TryParse(setValues[i + 1], out decimal salary))
                            {
                                throw new ArgumentException("Incorrect salary", nameof(setValues));
                            }

                            recordData = new RecordData() { Salary = salary };
                            this.validator.ValidatePrameter("salary", recordData);
                            foundResult[j].Salary = salary;
                            break;
                        default: throw new ArgumentException("Incorrect format", nameof(setValues));
                    }
                }
            }
        }

        private string GetFilterString(string[] values)
        {
            string filterString = string.Empty;
            for (int i = 0; i < values.Length; i++)
            {
                filterString += values[i];
            }

            return filterString.ToUpperInvariant();
        }

        private bool Remove(int id)
        {
            int i = FindRecord(this.list, id);
            if (i == this.list.Count)
            {
                return false;
            }

            FileCabinetRecord record = this.list[i];
            this.list.RemoveAt(i);

            RemoveFromDictionary(this.firstNameDictionary, record.FirstName.ToUpperInvariant(), id);
            RemoveFromDictionary(this.lastNameDictionary, record.LastName.ToUpperInvariant(), id);
            RemoveFromDictionary(this.dateOfBirthDictionary, record.DateOfBirth.ToString(this.culture), id);
            this.presentIdList.Remove(id);

            return true;
        }

        private void AddToDictionary(Dictionary<string, List<FileCabinetRecord>> dictionary, string name, int id)
        {
            List<FileCabinetRecord> list;
            string nameStr = name.ToUpperInvariant();
            if (dictionary.ContainsKey(nameStr))
            {
                dictionary[nameStr].Add(this.list[this.presentIdList[id]]);
            }
            else
            {
                list = new List<FileCabinetRecord>
                {
                    this.list[this.presentIdList[id]],
                };
                dictionary.Add(nameStr, list);
            }
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
                switch (values[i + 1].ToUpperInvariant())
                {
                    case "ID":
                        if (!int.TryParse(values[i + 2], out int id))
                        {
                            throw new ArgumentException("Incorrect id", nameof(values));
                        }

                        if (string.Equals(values[i], "AND", StringComparison.InvariantCultureIgnoreCase))
                        {
                            modifiedList = modifiedList.Intersect(FindById(modifiedList, id));
                        }
                        else
                        {
                            if (string.Equals(values[i], "OR", StringComparison.InvariantCultureIgnoreCase))
                            {
                                modifiedList = modifiedList.Union(FindById(this.list, id));
                            }
                            else
                            {
                                if (i == 0 && string.Equals(values[i], "WHERE", StringComparison.InvariantCultureIgnoreCase))
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
                    case "FIRSTNAME":
                        string firstnameStr = values[i + 2].ToUpperInvariant();
                        if (this.firstNameDictionary.ContainsKey(firstnameStr))
                        {
                            if (string.Equals(values[i], "AND", StringComparison.InvariantCultureIgnoreCase))
                            {
                                modifiedList = modifiedList.Intersect(this.firstNameDictionary[firstnameStr]);
                            }
                            else
                            {
                                if (string.Equals(values[i], "OR", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    modifiedList = modifiedList.Union(this.firstNameDictionary[firstnameStr]);
                                }
                                else
                                {
                                    if (i == 0 && string.Equals(values[i], "WHERE", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        modifiedList = this.firstNameDictionary[firstnameStr];
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Incorrect format", nameof(values));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (string.Equals(values[i], "AND", StringComparison.InvariantCultureIgnoreCase)
                                || (i == 0 && string.Equals(values[i], "WHERE", StringComparison.InvariantCultureIgnoreCase)))
                            {
                                modifiedList = new List<FileCabinetRecord>();
                            }
                        }

                        break;
                    case "LASTNAME":
                        string lastnameStr = values[i + 2].ToUpperInvariant();
                        if (this.lastNameDictionary.ContainsKey(lastnameStr))
                        {
                            if (string.Equals(values[i], "AND", StringComparison.InvariantCultureIgnoreCase))
                            {
                                modifiedList = modifiedList.Intersect(this.lastNameDictionary[lastnameStr]);
                            }
                            else
                            {
                                if (string.Equals(values[i], "OR", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    modifiedList = modifiedList.Union(this.lastNameDictionary[lastnameStr]);
                                }
                                else
                                {
                                    if (i == 0 && string.Equals(values[i], "WHERE", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        modifiedList = this.lastNameDictionary[lastnameStr];
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Incorrect format", nameof(values));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (string.Equals(values[i], "AND", StringComparison.InvariantCultureIgnoreCase)
                                || (i == 0 && string.Equals(values[i], "WHERE", StringComparison.InvariantCultureIgnoreCase)))
                            {
                                modifiedList = new List<FileCabinetRecord>();
                            }
                        }

                        break;
                    case "DATEOFBIRTH":
                        string dateStr = values[i + 2].ToUpperInvariant();
                        if (this.dateOfBirthDictionary.ContainsKey(dateStr))
                        {
                            if (string.Equals(values[i], "AND", StringComparison.InvariantCultureIgnoreCase))
                            {
                                modifiedList = modifiedList.Intersect(this.dateOfBirthDictionary[dateStr]);
                            }
                            else
                            {
                                if (string.Equals(values[i], "OR", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    modifiedList = modifiedList.Union(this.dateOfBirthDictionary[dateStr]);
                                }
                                else
                                {
                                    if (i == 0 && string.Equals(values[i], "WHERE", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        modifiedList = this.dateOfBirthDictionary[dateStr];
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Incorrect format", nameof(values));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (string.Equals(values[i], "AND", StringComparison.InvariantCultureIgnoreCase) || (i == 0 && values[i] == "where"))
                            {
                                modifiedList = new List<FileCabinetRecord>();
                            }
                        }

                        break;
                    case "GENDER":
                        if (!char.TryParse(values[i + 2], out char gender))
                        {
                            throw new ArgumentException("Incorrect gender", nameof(values));
                        }

                        if (string.Equals(values[i], "AND", StringComparison.InvariantCultureIgnoreCase))
                        {
                            modifiedList = modifiedList.Intersect(FindByGender(modifiedList, gender));
                        }
                        else
                        {
                            if (string.Equals(values[i], "OR", StringComparison.InvariantCultureIgnoreCase))
                            {
                                modifiedList = modifiedList.Union(FindByGender(this.list, gender));
                            }
                            else
                            {
                                if (i == 0 && string.Equals(values[i], "WHERE", StringComparison.InvariantCultureIgnoreCase))
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
                    case "PASSPORTID":
                        if (!short.TryParse(values[i + 2], out short passportId))
                        {
                            throw new ArgumentException("Incorrect passportId", nameof(values));
                        }

                        if (string.Equals(values[i], "AND", StringComparison.InvariantCultureIgnoreCase))
                        {
                            modifiedList = modifiedList.Intersect(FindByPassportId(modifiedList, passportId));
                        }
                        else
                        {
                            if (string.Equals(values[i], "OR", StringComparison.InvariantCultureIgnoreCase))
                            {
                                modifiedList = modifiedList.Union(FindByPassportId(this.list, passportId));
                            }
                            else
                            {
                                if (i == 0 && string.Equals(values[i], "WHERE", StringComparison.InvariantCultureIgnoreCase))
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
                    case "SALARY":
                        if (!decimal.TryParse(values[i + 2], out decimal salary))
                        {
                            throw new ArgumentException("Incorrect passportId", nameof(values));
                        }

                        if (string.Equals(values[i], "AND", StringComparison.InvariantCultureIgnoreCase))
                        {
                            modifiedList = modifiedList.Intersect(FindBySalary(modifiedList, salary));
                        }
                        else
                        {
                            if (string.Equals(values[i], "OR", StringComparison.InvariantCultureIgnoreCase))
                            {
                                modifiedList = modifiedList.Union(FindBySalary(this.list, salary));
                            }
                            else
                            {
                                if (i == 0 && string.Equals(values[i], "WHERE", StringComparison.InvariantCultureIgnoreCase))
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
