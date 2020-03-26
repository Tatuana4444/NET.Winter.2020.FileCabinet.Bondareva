using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class DefaultGenderValidator : IRecordValidator
    {
        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.Gender != 'W' && recordData.Gender != 'M')
            {
                throw new ArgumentException("Gender should be W or M", nameof(recordData));
            }
        }
    }
}
