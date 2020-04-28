using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Check data by validators.
    /// </summary>
    public class CompositeValidator : IValidator
    {
        private Dictionary<string, IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Validators.</param>
        public CompositeValidator(Dictionary<string, IRecordValidator> validators)
        {
            this.validators = validators;
        }

        /// <summary>
        /// Check data by validator.
        /// </summary>
        /// <param name="validator">Name of Validator.</param>
        /// <param name="recordData">Checking data.</param>
        public void ValidatePrameter(string validator, RecordData recordData)
        {
            this.validators[validator].ValidateParameters(recordData);
        }

        /// <summary>
        /// Check data by validators.
        /// </summary>
        /// <param name="recordData">Check data.</param>
        public void ValidateParameters(RecordData recordData)
        {
            ICollection<string> keys = this.validators.Keys;
            foreach (var k in keys)
            {
                this.validators[k].ValidateParameters(recordData);
            }
        }
    }
}
