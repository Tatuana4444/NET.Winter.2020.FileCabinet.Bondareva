using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Reader for xml files.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private XmlReader stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="stream">Current Stream.</param>
        public FileCabinetRecordXmlReader(StreamReader stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;
            this.stream = XmlReader.Create(stream, settings);
        }

        /// <summary>
        /// Read all records from stream.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            var validator = new ValidatorBuilder().CreateDefault();

            XmlSerializer ser = new XmlSerializer(typeof(List<FileCabinetRecord>));
            list = (List<FileCabinetRecord>)ser.Deserialize(this.stream);
            int i = 0;
            while (i < list.Count)
            {
                try
                {
                    RecordData data = new RecordData(list[i].FirstName, list[i].LastName, list[i].DateOfBirth, list[i].Gender, list[i].PassportId, list[i].Salary);
                    validator.ValidateParameters(data);
                    i++;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine(e.Message);
                    list.RemoveAt(i);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    list.RemoveAt(i);
                }
            }

            return list;
        }
    }
}
