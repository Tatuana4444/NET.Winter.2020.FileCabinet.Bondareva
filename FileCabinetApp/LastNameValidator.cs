using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class LastNameValidator : IRecordValidator
    {
        private int minLength;

        private int maxLength;

        public LastNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        public void ValidateParametrs(RecordData recordData)
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
