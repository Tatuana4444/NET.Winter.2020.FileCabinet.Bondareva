using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Record for serializer.
    /// </summary>
    [XmlRoot("records")]
    public class RecordForSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordForSerializer"/> class.
        /// </summary>
        public RecordForSerializer()
        {
            this.Record = new List<Record>();
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        [XmlElement("record")]
        public List<Record> Record { get; }
    }
}
