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
        /// Returns records.
        /// </summary>
        /// <param name="filter">Record's filter. Filter start from 'where' and can contain 'and' and 'or'.</param>
        /// <returns>Records by filret.</returns>
        public ReadOnlyCollection<FileCabinetRecord> SelectRecords(string filter);

        /// <summary>
        /// Returns count of records.
        /// </summary>
        /// <returns>Count of records and count of deleted records.</returns>
        public Tuple<int, int> GetStat();

        /// <summary>
        /// Restore data from snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot.</param>
        void Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Defragment the data file.
        /// </summary>
        /// <returns>Count of defragmented records.</returns>
        int Purge();

        /// <summary>
        /// Delete records by parameters.
        /// </summary>
        /// <param name="param">Record parameters.</param>
        /// <returns>List of id recored, that was deleted.</returns>
        IEnumerable<int> Delete(string param);

        /// <summary>
        /// Update records by parameters.
        /// </summary>
        /// <param name="param">Record parameters.</param>
        void Update(string param);

        /// <summary>
        /// Create new snapshot.
        /// </summary>
        /// <returns>Snapshot.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();
    }
}
