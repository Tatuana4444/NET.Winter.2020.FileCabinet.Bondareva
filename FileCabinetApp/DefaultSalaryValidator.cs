using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class DefaultSalaryValidator : IRecordValidator
    {
        /// <summary>
        /// Min salary for Belarus.
        /// </summary>
        public const int MinSalary = 375;

        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.Salary < MinSalary)
            {
                throw new ArgumentException($"Salary can't be less than {MinSalary}", nameof(recordData));
            }
        }
    }
}
