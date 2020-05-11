using System;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Serializes records to csv file.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">Writer for text.</param>
        /// <exception cref="ArgumentNullException">Thrown when writer is null.</exception>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer), "Writer can't be null");
            }

            this.writer = writer;
        }

        /// <summary>
        /// Writes records to csv file.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record can't be null.");
            }

            this.writer.WriteLine($"{record.Id}, {record.FirstName}, {record.LastName}, " +
                $"{record.DateOfBirth}, {record.Gender}, {record.PassportId}, {record.Salary}");
        }
    }
}
