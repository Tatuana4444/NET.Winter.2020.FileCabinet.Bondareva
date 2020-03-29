using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface for FileCabinetService.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Create a new record.
        /// </summary>
        /// <param name="recordData">User's data.</param>
        /// <returns>Id of a new record.</returns>
        public int CreateRecord(RecordData recordData);

        /// <summary>
        /// Returns all records.
        /// </summary>
        /// <returns>All records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Returns count of records.
        /// </summary>
        /// <returns>Count of records and count of deleted records.</returns>
        public Tuple<int, int> GetStat();

        /// <summary>
        /// Edit record by id.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <param name="recordData">User's data.</param>
        public void EditRecord(int id, RecordData recordData);

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <returns>Records whith sought-for firstName.</returns>
        public IRecordIterator FindByFirstName(string firstName);

        /// <summary>
        /// Finds records by last name.
        /// </summary>
        /// <param name="lastName">User's last name.</param>
        /// <returns>Records whith sought-for lastName.</returns>
        public IRecordIterator FindByLastName(string lastName);

        /// <summary>
        /// Finds records by DateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <returns>Records whith sought-for date of Birth.</returns>
        public IRecordIterator FindByDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Restore data from snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot.</param>
        void Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Remove record by id.
        /// </summary>
        /// <param name="id">Id record.</param>
        /// <returns>True, if record exists, otherway returns false.</returns>
        bool Remove(int id);

        /// <summary>
        /// Defragment the data file.
        /// </summary>
        /// <returns>Count of defragmented records.</returns>
        int Purge();
    }
}
