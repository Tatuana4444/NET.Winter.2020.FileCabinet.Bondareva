using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetService with logger.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private readonly IFileCabinetService service;
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">Service that needs to use.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = service;
            this.writer = File.CreateText("log.txt");
        }

        /// <summary>
        /// Create a new record and print tick that it took.
        /// </summary>
        /// <param name="recordData">User's data.</param>
        /// <returns>Id of a new record.</returns>
        /// <exception cref="ArgumentNullException">Throw when recordData is null.</exception>
        public int CreateRecord(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Record data can't be null.");
            }

            this.LogWriter($"{DateTime.Now} - Calling Create() with FirstName = '{recordData.FirstName}', " +
                $"LastName = '{recordData.LastName}', DateOfBirth = '{recordData.DateOfBirth.ToString("yyyy - MMM - dd", Culture)}', " +
                $"Gender = '{recordData.Gender}', PassportId = '{recordData.PassportId}', " +
                $"Salary = '{recordData.Salary}'");

            try
            {
                var result = this.service.CreateRecord(recordData);
                this.LogWriter($"{DateTime.Now} - Create() returned {result}");
                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - Create() throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Delete record by parameters.
        /// </summary>
        /// <param name="param">Record parameters.</param>
        /// <returns>List of id records, that was deleted.</returns>
        public IEnumerable<int> Delete(string param)
        {
            this.LogWriter($"{DateTime.Now} - Calling Delete() with parameters = '{param}'");

            try
            {
                var result = this.service.Delete(param);
                this.LogWriter($"{DateTime.Now} - Delete() returned RemovedId = '{result}'");
                foreach (int id in result)
                {
                    this.LogWriter($"Id = '{id}','");
                }

                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - Delete() throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets count of records  and count of deleted records and print tick that it took.
        /// </summary>
        /// <returns>Count of records  and count of deleted records.</returns>
        public Tuple<int, int> GetStat()
        {
            this.LogWriter($"{DateTime.Now} - Calling GetStat()");

            try
            {
                var result = this.service.GetStat();
                this.LogWriter($"{DateTime.Now} - GetStat() returned CountOfRecords = '{result.Item1}', CountOfDeletedRecords = '{result.Item2}'");

                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - GetStat() throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create new snapshot.
        /// </summary>
        /// <returns>Snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.LogWriter($"{DateTime.Now} - Calling MakeSnapshot()");

            try
            {
                var result = this.service.MakeSnapshot();
                this.LogWriter($"{DateTime.Now} - MakeSnapshot() returned Snapshot with records:");
                foreach (var r in result.Records)
                {
                    this.LogWriter($"Id = '{r.Id}', FirstName = '{r.FirstName}', " +
                        $"LastName = '{r.LastName}', DateOfBirth = '{r.DateOfBirth.ToString("yyyy - MMM - dd", Culture)}', " +
                        $"Gender = '{r.Gender}', PassportId = '{r.PassportId}', " +
                        $"Salary = '{r.Salary}'");
                }

                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - GetStat() throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Defragment the data file and print tick that it took.
        /// </summary>
        /// <returns>Count of defragmented records.</returns>
        public int Purge()
        {
            this.LogWriter($"{DateTime.Now} - Calling Purge()");

            try
            {
                var result = this.service.Purge();
                this.LogWriter($"{DateTime.Now} - Purge() returned CountOfDeletedRecords = '{result}'");

                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - Purge() throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Restore date from snapshot and print tick that it took.
        /// </summary>
        /// <param name="snapshot">Snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                this.LogWriter($"{DateTime.Now} - Calling Restore() with Snapshot = 'null'");
                this.LogWriter($"{DateTime.Now} - Restore() throw Snapshot can't be null");
                throw new ArgumentNullException(nameof(snapshot), "Snapshot can't be null");
            }
            else
            {
                this.LogWriter($"{DateTime.Now} - Calling Restore() with Snapshot: ");
            }

            foreach (var r in snapshot.Records)
            {
                this.LogWriter($"Id = '{r.Id}', FirstName = '{r.FirstName}', " +
                    $"LastName = '{r.LastName}', DateOfBirth = '{r.DateOfBirth.ToString("yyyy - MMM - dd", Culture)}', " +
                    $"Gender = '{r.Gender}', PassportId = '{r.PassportId}', " +
                    $"Salary = '{r.Salary}'");
            }

            try
            {
                this.service.Restore(snapshot);
                this.LogWriter($"{DateTime.Now} - Restore() finished.");
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - Restore() throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Returns records.
        /// </summary>
        /// <param name="filter">Record's filter. Filter start from 'where' and can contain 'and' and 'or'.</param>
        /// <returns>Records by filter.</returns>
        public ReadOnlyCollection<FileCabinetRecord> SelectRecords(string filter)
        {
            this.LogWriter($"{DateTime.Now} - Calling SelectRecords()");

            try
            {
                var result = this.service.SelectRecords(filter);
                this.LogWriter($"{DateTime.Now} - SelectRecords() returned: ");
                foreach (var r in result)
                {
                    this.LogWriter($"Id = '{r.Id}', FirstName = '{r.FirstName}', " +
                        $"LastName = '{r.LastName}', DateOfBirth = '{r.DateOfBirth.ToString("yyyy - MMM - dd", Culture)}', " +
                        $"Gender = '{r.Gender}', PassportId = '{r.PassportId}', " +
                        $"Salary = '{r.Salary}'");
                }

                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - SelectRecords() throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Update records by parameters.
        /// </summary>
        /// <param name="param">Record parameters.</param>
        public void Update(string param)
        {
            if (param is null)
            {
                throw new ArgumentNullException(nameof(param), "Record data can't be null.");
            }

            this.LogWriter($"{DateTime.Now} - Calling Update() with id = '{param}'");

            try
            {
                this.service.Update(param);
                this.LogWriter($"{DateTime.Now} - Update() finished.");
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - Update() throw {ex.Message}");
                throw;
            }
        }

        private void LogWriter(string text)
        {
            this.writer.WriteLine(text);
            this.writer.Flush();
        }
    }
}
