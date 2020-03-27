using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class SalaryValidator : IRecordValidator
    {
        private decimal minSalary;

        public SalaryValidator(decimal minSalary)
        {
            this.minSalary = minSalary;
        }

        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.Salary < this.minSalary)
            {
                throw new ArgumentException($"Salary can't be less than {this.minSalary}", nameof(recordData));
            }
        }
    }
}
