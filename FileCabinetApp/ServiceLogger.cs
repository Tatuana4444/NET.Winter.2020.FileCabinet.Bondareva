﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetService with logger.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private static CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");
        private static DateTimeFormatInfo dtfi = englishUS.DateTimeFormat;
        private IFileCabinetService service;
        private TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">Service that needs to use.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = new ServiceMeter(service);
            dtfi.ShortDatePattern = "yyyy-MMM-dd";
            this.writer = File.CreateText("log.txt");
        }

        /// <summary>
        /// Create a new record and print tick that it took.
        /// </summary>
        /// <param name="recordData">User's data.</param>
        /// <returns>Id of a new record.</returns>
        public int CreateRecord(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Record data can't be null.");
            }

            this.LogWriter($"{DateTime.Now} - Calling Create() with FirstName = '{recordData.FirstName}', " +
                $"LastName = '{recordData.LastName}', DateOfBirth = '{recordData.DateOfBirth.ToString(englishUS)}', " +
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
        /// Edit record by id and print tick that it took.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <param name="recordData">User's data.</param>
        public void EditRecord(int id, RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Record data can't be null.");
            }

            this.LogWriter($"{DateTime.Now} - Calling Edit() with id = '{id}', FirstName = '{recordData.FirstName}', " +
                $"LastName = '{recordData.LastName}', DateOfBirth = '{recordData.DateOfBirth.ToString(englishUS)}', " +
                $"Gender = '{recordData.Gender}', PassportId = '{recordData.PassportId}', " +
                $"Salary = '{recordData.Salary}'");

            try
            {
                this.service.EditRecord(id, recordData);
                this.LogWriter($"{DateTime.Now} - Edit() finished.");
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - Edit() throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Finds records by DateOfBirth and print tick that it took.
        /// </summary>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <returns>Records whith sought-for date of Birth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            this.LogWriter($"{DateTime.Now} - Calling Find() with DateOfBirth = '{dateOfBirth.ToString(englishUS)}'");

            try
            {
                var result = this.service.FindByDateOfBirth(dateOfBirth);
                this.LogWriter($"{DateTime.Now} - Find() by dateOfBirth returned:");
                foreach (var r in result)
                {
                    this.LogWriter($"Id = '{r.Id}', FirstName = '{r.FirstName}', " +
                        $"LastName = '{r.LastName}', DateOfBirth = '{r.DateOfBirth.ToString(englishUS)}', " +
                        $"Gender = '{r.Gender}', PassportId = '{r.PassportId}', " +
                        $"Salary = '{r.Salary}'");
                }

                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - Find() by dateOfBirth throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Finds records by first name and print tick that it took.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <returns>Records whith sought-for firstName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.LogWriter($"{DateTime.Now} - Calling Find() with FirstName = '{firstName}'");

            try
            {
                var result = this.service.FindByFirstName(firstName);
                this.LogWriter($"{DateTime.Now} - Find() by FirstName returned:");
                foreach (var r in result)
                {
                    this.LogWriter($"Id = '{r.Id}', FirstName = '{r.FirstName}', " +
                        $"LastName = '{r.LastName}', DateOfBirth = '{r.DateOfBirth.ToString(englishUS)}', " +
                        $"Gender = '{r.Gender}', PassportId = '{r.PassportId}', " +
                        $"Salary = '{r.Salary}'");
                }

                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - Find() by FirstName throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Finds records by last name and print tick that it took.
        /// </summary>
        /// <param name="lastName">User's last name.</param>
        /// <returns>Records whith sought-for lastName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.LogWriter($"{DateTime.Now} - Calling Find() with LastName = '{lastName}'");

            try
            {
                var result = this.service.FindByLastName(lastName);
                this.LogWriter($"{DateTime.Now} - Find() by LastName returned:");
                foreach (var r in result)
                {
                    this.LogWriter($"Id = '{r.Id}', FirstName = '{r.FirstName}', " +
                        $"LastName = '{r.LastName}', DateOfBirth = '{r.DateOfBirth.ToString(englishUS)}', " +
                        $"Gender = '{r.Gender}', PassportId = '{r.PassportId}', " +
                        $"Salary = '{r.Salary}'");
                }

                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - Find() by LastName throw {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets all records and print tick that it took.
        /// </summary>
        /// <returns>All records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.LogWriter($"{DateTime.Now} - Calling GetRecords()");

            try
            {
                var result = this.service.GetRecords();
                this.LogWriter($"{DateTime.Now} - GetRecords() returned: ");
                foreach (var r in result)
                {
                    this.LogWriter($"Id = '{r.Id}', FirstName = '{r.FirstName}', " +
                        $"LastName = '{r.LastName}', DateOfBirth = '{r.DateOfBirth.ToString(englishUS)}', " +
                        $"Gender = '{r.Gender}', PassportId = '{r.PassportId}', " +
                        $"Salary = '{r.Salary}'");
                }

                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - GetRecords() throw {ex.Message}");
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
        /// Remove record by id and print tick that it took.
        /// </summary>
        /// <param name="id">Id record.</param>
        /// <returns>True, if record exists, otherway returns false.</returns>
        public bool Remove(int id)
        {
            this.LogWriter($"{DateTime.Now} - Calling Remove() with Id = '{id}'");

            try
            {
                var result = this.service.Remove(id);
                this.LogWriter($"{DateTime.Now} - Remove() returned IsRemoved = '{result}'");

                return result;
            }
            catch (Exception ex)
            {
                this.LogWriter($"{DateTime.Now} - Remove() throw {ex.Message}");
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
                this.LogWriter($"{DateTime.Now} - Calling Restore() with Snaphot = 'null'");
                this.LogWriter($"{DateTime.Now} - Restore() throw Snapshot can't be null");
                throw new ArgumentNullException(nameof(snapshot), "Snapshot can't be null");
            }
            else
            {
                this.LogWriter($"{DateTime.Now} - Calling Restore() with Snaphot: ");
            }

            foreach (var r in snapshot.Records)
            {
                this.LogWriter($"Id = '{r.Id}', FirstName = '{r.FirstName}', " +
                    $"LastName = '{r.LastName}', DateOfBirth = '{r.DateOfBirth.ToString(englishUS)}', " +
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

        private void LogWriter(string text)
        {
            this.writer.WriteLine(text);
            this.writer.Flush();
        }
    }
}
