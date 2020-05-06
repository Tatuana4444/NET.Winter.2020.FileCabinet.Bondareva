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
        private readonly XmlReader stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="stream">Current Stream.</param>
        public FileCabinetRecordXmlReader(StreamReader stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                Async = true,
            };
            this.stream = XmlReader.Create(stream, settings);
        }

        /// <summary>
        /// Read all records from stream.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();
            RecordForSerializer list;

            var validator = new ValidatorBuilder().CreateDefault();

            XmlSerializer ser = new XmlSerializer(typeof(RecordForSerializer));
            try
            {
                list = (RecordForSerializer)ser.Deserialize(this.stream);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }

            int i = 0;
            foreach (var record in list.Record)
            {
                try
                {
                    RecordData data = new RecordData(record.Name.FirstName, record.Name.LastName, record.DateOfBirth, record.Gender, record.PassportId, record.Salary);
                    validator.ValidateParameters(data);
                    result.Add(new FileCabinetRecord()
                    {
                        Id = record.Id,
                        FirstName = record.Name.FirstName,
                        LastName = record.Name.LastName,
                        DateOfBirth = record.DateOfBirth,
                        Gender = record.Gender,
                        PassportId = record.PassportId,
                        Salary = record.Salary,
                    });
                    i++;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return result;
        }
    }
}
