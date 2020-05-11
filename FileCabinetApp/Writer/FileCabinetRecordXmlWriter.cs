using System;
using System.Globalization;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Serializes records to xml file.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter writer;
        private readonly CultureInfo culture;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">Writer for text.</param>
        /// <exception cref="ArgumentNullException">Thrown when writer is null.</exception>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer), "Writer can't be null");
            }

            this.writer = writer;
            this.writer.WriteStartDocument();
            this.writer.WriteStartElement("records");
            this.culture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Writes records to xml file.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record can't be null.");
            }

            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", record.Id.ToString(this.culture));
            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", record.FirstName);
            this.writer.WriteAttributeString("last", record.LastName);
            this.writer.WriteEndElement();
            this.writer.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("yyyy-MM-dd", this.culture));
            this.writer.WriteElementString("gender", record.Gender.ToString(this.culture));
            this.writer.WriteElementString("passportId", record.PassportId.ToString(this.culture));
            this.writer.WriteElementString("salary", record.Salary.ToString(this.culture));
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
