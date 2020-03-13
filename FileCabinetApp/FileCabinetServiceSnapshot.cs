using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Snapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records = Array.Empty<FileCabinetRecord>();

        /// <summary>
        /// Gets records.
        /// </summary>
        /// <value>
        /// Records.
        /// </value>
        public IReadOnlyCollection<FileCabinetRecord> Records => new ReadOnlyCollection<FileCabinetRecord>(this.records);

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

        /// <summary>
        /// Save records to xml file.
        /// </summary>
        /// <param name="streamWriter">Stream for writing.</param>
        public void SaveToXml(StreamWriter streamWriter)
        {
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.OmitXmlDeclaration = true;
            writerSettings.NewLineOnAttributes = false;
            var cabinetRecordXmlWriter = new FileCabinetRecordXmlWriter(XmlWriter.Create(streamWriter, writerSettings));
            foreach (var record in this.records)
            {
                cabinetRecordXmlWriter.Write(record);
            }

            cabinetRecordXmlWriter.EndSaving();
        }

        /// <summary>
        /// Loads records from csv file.
        /// </summary>
        /// <param name="stream">Current stream.</param>
        /// <returns>Count loaded records.</returns>
        internal int LoadFromCsv(StreamReader stream)
        {
            FileCabinetRecordCsvReader reader = new FileCabinetRecordCsvReader(stream);
            this.records = reader.ReadAll().ToArray();
            return this.records.Length;
        }
    }
}
