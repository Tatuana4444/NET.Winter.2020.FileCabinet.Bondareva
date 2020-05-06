using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for salary.
    /// </summary>
    public class SalaryValidator : IRecordValidator
    {
        private readonly decimal minSalary;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="minSalary">Min value for salary.</param>
        public SalaryValidator(decimal minSalary)
        {
            this.minSalary = minSalary;
        }

        /// <summary>
        /// Check salary from data.
        /// </summary>
        /// <param name="recordData">Data.</param>
        public void ValidateParameters(RecordData recordData)
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
