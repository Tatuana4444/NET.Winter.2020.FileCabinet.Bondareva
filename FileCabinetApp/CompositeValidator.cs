using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Check data by validators.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Validators.</param>
        public CompositeValidator(List<IRecordValidator> validators)
        {
            this.validators = validators;
        }

        /// <summary>
        /// Check data by validators.
        /// </summary>
        /// <param name="recordData">Check data.</param>
        public void ValidateParametrs(RecordData recordData)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParametrs(recordData);
            }
        }
    }
}
