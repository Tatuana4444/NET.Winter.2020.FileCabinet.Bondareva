using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for last name.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly int minLength;

        private readonly int maxLength;

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
        /// <exception cref="ArgumentNullException">Throw when recordData, recordData.LastName is null.</exception>
        /// <exception cref="ArgumentException">Throw when LastName length more than maxLength or less than minLength.</exception>
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

            if (recordData.LastName.Trim().Length < this.minLength || recordData.LastName.Trim().Length > this.maxLength)
            {
                throw new ArgumentException($"Length of last name can't be less than {this.minLength} and more than {this.maxLength}", nameof(recordData));
            }
        }
    }
}
