using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class CustomDateOfBirthValidator : IRecordValidator
    {
        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if ((DateTime.Compare(new DateTime(1900, 1, 1), recordData.DateOfBirth) > 0)
                || (DateTime.Compare(DateTime.Now, recordData.DateOfBirth) < 0))
            {
                throw new ArgumentException("Date of Birth can't be less than 01-Jan-1900 and more than today", nameof(recordData));
            }
        }
    }
}
