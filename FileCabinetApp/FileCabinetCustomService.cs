using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Implements Custom FileCabinetService.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Implements Custom Validate Parametrs.
        /// </summary>
        /// <param name="recordData">User's data.</param>
        protected override void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.FirstName is null)
            {
                throw new ArgumentNullException(nameof(recordData), "First name can't be null");
            }

            if (recordData.FirstName.Length < 2 || recordData.FirstName.Length > 100)
            {
                throw new ArgumentException("Length of first name can't be less than 2 and more than 60", nameof(recordData));
            }

            if (recordData.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("First name can't contain only spaces", nameof(recordData));
            }

            if (recordData.LastName is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Last name can't be null");
            }

            if (recordData.LastName.Length < 2 || recordData.LastName.Length > 100)
            {
                throw new ArgumentException("Length of last name can't be less than 2 and more than 60", nameof(recordData));
            }

            if (recordData.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Last name can't contain only spaces", nameof(recordData));
            }

            if ((DateTime.Compare(new DateTime(1900, 1, 1), recordData.DateOfBirth) > 0)
                || (DateTime.Compare(DateTime.Now, recordData.DateOfBirth) < 0))
            {
                throw new ArgumentException("Date of Birth can't be less than 01-Jan-1950 and more than today", nameof(recordData));
            }

            if (recordData.Gender != 'W' && recordData.Gender != 'M')
            {
                throw new ArgumentException("Gender should be W or M", nameof(recordData));
            }

            if (recordData.PassportId < 0)
            {
                throw new ArgumentException("Passport Id can't be less than 1000 and more than 9999", nameof(recordData));
            }

            if (recordData.Salary < 0)
            {
                throw new ArgumentException($"Salary can't be less than {MinSalary}", nameof(recordData));
            }
        }
    }
}
