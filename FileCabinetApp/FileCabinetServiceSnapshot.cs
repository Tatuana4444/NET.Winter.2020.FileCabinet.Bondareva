using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Snapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records = Array.Empty<FileCabinetRecord>();

        /// <summary>
        /// Set records.
        /// </summary>
        /// <param name="records">Snapshot of records.</param>
        public void SetState(FileCabinetRecord[] records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records), "Records can't be null.");
            }

            this.records = records;
        }

        /// <summary>
        /// Save records to csv file.
        /// </summary>
        /// <param name="streamWriter">Stream for writing.</param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            var cabinetRecordCsvWriter = new FileCabinetRecordCsvWriter(streamWriter);
            foreach (var record in this.records)
            {
                cabinetRecordCsvWriter.Write(record);
            }
        }
    }
}
