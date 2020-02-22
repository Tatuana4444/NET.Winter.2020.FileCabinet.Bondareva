﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Validator.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Min salary for Belarus.
        /// </summary>
        public const int MinSalary = 375;

        /// <summary>
        /// DefaultValidator.
        /// </summary>
        /// <param name="recordData">User's data.</param>
        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.FirstName is null)
            {
                throw new ArgumentNullException(nameof(recordData), "First name can't be null");
            }

            if (recordData.FirstName.Length < 2 || recordData.FirstName.Length > 60)
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

            if (recordData.LastName.Length < 2 || recordData.LastName.Length > 60)
            {
                throw new ArgumentException("Length of last name can't be less than 2 and more than 60", nameof(recordData));
            }

            if (recordData.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Last name can't contain only spaces", nameof(recordData));
            }

            if ((DateTime.Compare(new DateTime(1950, 1, 1), recordData.DateOfBirth) > 0)
                || (DateTime.Compare(DateTime.Now, recordData.DateOfBirth) < 0))
            {
                throw new ArgumentException("Date of Birth can't be less than 01-Jan-1950 and more than today", nameof(recordData));
            }

            if (recordData.Gender != 'W' && recordData.Gender != 'M')
            {
                throw new ArgumentException("Gender should be W or M", nameof(recordData));
            }

            if (recordData.PassportId < 1000 || recordData.PassportId > 9999)
            {
                throw new ArgumentException("Passport Id can't be less than 1000 and more than 9999", nameof(recordData));
            }

            if (recordData.Salary < MinSalary)
            {
                throw new ArgumentException($"Salary can't be less than {MinSalary}", nameof(recordData));
            }
        }
    }
}
