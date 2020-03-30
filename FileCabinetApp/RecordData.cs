using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Implements Introduce Parameter Object.
    /// </summary>
    public class RecordData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordData"/> class.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <param name="lastName">User's last name.</param>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <param name="gender">User's gender.</param>
        /// <param name="passportId">User's passport id.</param>
        /// <param name="salary">User's salary.</param>
        public RecordData(string firstName, string lastName, DateTime dateOfBirth, char gender, short passportId, decimal salary)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Gender = gender;
            this.PassportId = passportId;
            this.Salary = salary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordData"/> class.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <param name="firstName">User's first name.</param>
        /// <param name="lastName">User's last name.</param>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <param name="gender">User's gender.</param>
        /// <param name="passportId">User's passport id.</param>
        /// <param name="salary">User's salary.</param>
        public RecordData(int id, string firstName, string lastName, DateTime dateOfBirth, char gender, short passportId, decimal salary)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Gender = gender;
            this.PassportId = passportId;
            this.Salary = salary;
        }

        /// <summary>
        /// Gets user's Id.
        /// </summary>
        /// <value>
        /// User's Id.
        /// </value>
        public int Id { get; private set; }

        /// <summary>
        /// Gets user's First name.
        /// </summary>
        /// <value>
        /// User's First name.
        /// </value>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets user's Last name.
        /// </summary>
        /// <value>
        /// User's Last name.
        /// </value>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets user's date of Birth.
        /// </summary>
        /// <value>
        /// User's date of Birth.
        /// </value>
        public DateTime DateOfBirth { get; private set; }

        /// <summary>
        /// Gets user's gender.
        /// </summary>
        /// <value>
        /// User's gender.
        /// </value>
        public char Gender { get; private set; }

        /// <summary>
        /// Gets user's passport id.
        /// </summary>
        /// <value>
        /// User's passport id.
        /// </value>
        public short PassportId { get; private set; }

        /// <summary>
        /// Gets user's salary.
        /// </summary>
        /// <value>
        /// User's salary.
        /// </value>
        public decimal Salary { get; private set; }
    }
}
