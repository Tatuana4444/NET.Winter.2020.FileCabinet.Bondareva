using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class CustomLastNameValidator : IRecordValidator
    {
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

            if (recordData.LastName.Length < 2 || recordData.LastName.Length > 100)
            {
                throw new ArgumentException("Length of last name can't be less than 2 and more than 60", nameof(recordData));
            }

            if (recordData.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Last name can't contain only spaces", nameof(recordData));
            }
        }
    }
}
