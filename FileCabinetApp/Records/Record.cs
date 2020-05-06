using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Record.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        public Record()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <param name="name">User's first name and last name.</param>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <param name="gender">User's gender.</param>
        /// <param name="passportId">User's passport id.</param>
        /// <param name="salary">User's salary.</param>
        public Record(int id, Name name, DateTime dateOfBirth, char gender, short passportId, decimal salary)
        {
            this.Id = id;
            this.Name = name;
            this.DateOfBirth = dateOfBirth;
            this.Gender = gender;
            this.PassportId = passportId;
            this.Salary = salary;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlElement("name")]
        public Name Name { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        [XmlIgnore]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        [XmlElement("dateOfBirth")]
        [Browsable(false)]
        public string DateOfBirthString
        {
            get { return this.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US")); }
            set { this.DateOfBirth = Convert.ToDateTime(value, CultureInfo.CreateSpecificCulture("en-US")); }
        }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        [XmlIgnore]
        public char Gender { get; set; }

        /// <summary>
        /// Gets or sets user's gender by string.
        /// </summary>
        /// <value>
        /// User's gender by string.
        /// </value>
        [XmlElement("gender")]
        [Browsable(false)]
        public string GenderString
        {
            get { return this.Gender.ToString(CultureInfo.CreateSpecificCulture("en-US")); }
            set { this.Gender = Convert.ToChar(value, CultureInfo.CreateSpecificCulture("en-US")); }
        }

        /// <summary>
        /// Gets or sets duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        [XmlElement("passportId")]
        public short PassportId { get; set; }

        /// <summary>
        /// Gets or sets credit sum.
        /// </summary>
        /// <value>
        /// Credit sum.
        /// </value>
        [XmlElement("salary")]
        public decimal Salary { get; set; }
    }
}
