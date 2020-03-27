using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class DateOfBirthValidator : IRecordValidator
    {
        private DateTime from;

        private DateTime to;

        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if ((DateTime.Compare(this.from, recordData.DateOfBirth) > 0)
                || (DateTime.Compare(this.to, recordData.DateOfBirth) < 0))
            {
                throw new ArgumentException("Date of Birth can't be less than 01-Jan-1900 and more than today", nameof(recordData));
            }
        }
    }
}
