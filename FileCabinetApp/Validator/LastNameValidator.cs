using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for last name.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private int minLength;

        private int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Min length for last name.</param>
        /// <param name="maxLength">Max length for last name.</param>
        public LastNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Check last name from data.
        /// </summary>
        /// <param name="recordData">Data.</param>
        public void ValidateParameters(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.LastName is null)
            {
                throw new ArgumentNullException(nameof(recordData), "Last name can't be null");
            }

            if (recordData.LastName.Length < this.minLength || recordData.LastName.Length > this.maxLength)
            {
                throw new ArgumentException($"Length of last name can't be less than {this.minLength} and more than {this.maxLength}", nameof(recordData));
            }

            if (recordData.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Last name can't contain only spaces", nameof(recordData));
            }
        }
    }
}
