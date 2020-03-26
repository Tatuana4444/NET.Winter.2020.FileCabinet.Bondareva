using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class CustomSalaryValidator : IRecordValidator
    {

        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.Salary < 0)
            {
                throw new ArgumentException($"Salary can't be less than 0", nameof(recordData));
            }
        }
    }
}
