using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class CustomFirstNameValidator : IRecordValidator
    {
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

            if (recordData.FirstName.Length < 2 || recordData.FirstName.Length > 100)
            {
                throw new ArgumentException("Length of first name can't be less than 2 and more than 60", nameof(recordData));
            }

            if (recordData.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("First name can't contain only spaces", nameof(recordData));
            }
        }
    }
}
