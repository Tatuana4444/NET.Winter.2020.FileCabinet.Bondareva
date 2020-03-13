using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Describes record.
    /// </summary>
    [XmlRoot("Record")]
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets user's id.
        /// </summary>
        /// <value>
        /// User's id.
        /// </value>
        [XmlAttribute]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets user's First name.
        /// </summary>
        /// <value>
        /// User's First name.
        /// </value>
        [XmlElement]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets user's Last name.
        /// </summary>
        /// <value>
        /// User's Last name.
        /// </value>
        [XmlElement]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets user's date of Birth.
        /// </summary>
        /// <value>
        /// User's date of Birth.
        /// </value>
        [XmlElement]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets user's gender.
        /// </summary>
        /// <value>
        /// User's gender.
        /// </value>
        [XmlElement]
        public char Gender { get; set; }

        /// <summary>
        /// Gets or sets user's passport id.
        /// </summary>
        /// <value>
        /// User's passport id.
        /// </value>
        [XmlElement]
        public short PassportId { get; set; }

        /// <summary>
        /// Gets or sets user's salary.
        /// </summary>
        /// <value>
        /// User's salary.
        /// </value>
        [XmlElement]
        public decimal Salary { get; set; }
    }
}
