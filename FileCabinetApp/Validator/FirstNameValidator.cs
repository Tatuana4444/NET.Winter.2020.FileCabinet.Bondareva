﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Validators for first name.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int minLength;

        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Min length for first name.</param>
        /// <param name="maxLength">Max length for last name.</param>
        public FirstNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Check first name from data.
        /// </summary>
        /// <param name="recordData">Data.</param>
        public void ValidateParameters(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.FirstName is null)
            {
                throw new ArgumentNullException(nameof(recordData), "First name can't be null");
            }

            if (recordData.FirstName.Length < this.minLength || recordData.FirstName.Length > this.maxLength)
            {
                throw new ArgumentException($"Length of first name can't be less than {this.minLength} and more than {this.maxLength}", nameof(recordData));
            }

            if (recordData.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("First name can't contain only spaces", nameof(recordData));
            }
        }
    }
}
