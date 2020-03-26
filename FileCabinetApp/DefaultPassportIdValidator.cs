using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class DefaultPassportIdValidator : IRecordValidator
    {
        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.PassportId < 1000 || recordData.PassportId > 9999)
            {
                throw new ArgumentException("Passport Id can't be less than 1000 and more than 9999", nameof(recordData));
            }
        }
    }
}
