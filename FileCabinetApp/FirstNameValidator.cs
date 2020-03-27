using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class FirstNameValidator : IRecordValidator
    {
        private int minLength;

        private int maxLength;

        public FirstNameValidator(int minLength, int maxLength)
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
