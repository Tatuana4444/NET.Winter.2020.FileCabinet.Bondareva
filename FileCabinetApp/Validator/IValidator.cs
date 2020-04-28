using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Inferface for validator.
    /// </summary>
    public interface IValidator : IRecordValidator
    {
        /// <summary>
        /// Validate data by parameter.
        /// </summary>
        /// <param name="validator">Name of Validator.</param>
        /// <param name="recordData">User's data.</param>
        public void ValidatePrameter(string validator, RecordData recordData);
    }
}
