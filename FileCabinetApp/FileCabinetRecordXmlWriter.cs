using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Serializes records to xml file.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter writer;
        private readonly CultureInfo englishUS;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">Writer for text.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer), "Writer can't be null");
            }

            this.writer = writer;
            this.writer.WriteStartDocument();
            this.writer.WriteStartElement("records");
            this.englishUS = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeFormatInfo dtfi = this.englishUS.DateTimeFormat;
            dtfi.ShortDatePattern = "yyyy-MMM-dd";
        }

        /// <summary>
        /// Writes records to xml file.
        /// </summary>
        /// <param name="record">Record.</param>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record can't be null.");
            }

            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", record.Id.ToString(this.englishUS));
            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", record.FirstName);
            this.writer.WriteAttributeString("last", record.LastName);
            this.writer.WriteEndElement();
            this.writer.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("yyyy-MM-dd", this.englishUS));
            this.writer.WriteElementString("gender", record.Gender.ToString(this.englishUS));
            this.writer.WriteElementString("passportId", record.PassportId.ToString(this.englishUS));
            this.writer.WriteElementString("salary", record.Salary.ToString(this.englishUS));
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// End saving to the file.
        /// </summary>
        public void EndSaving()
        {
            this.writer.WriteEndDocument();
            this.writer.Flush();
        }
    }
}
