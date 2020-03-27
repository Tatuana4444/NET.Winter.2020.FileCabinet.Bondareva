using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class CompositeValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        protected CompositeValidator(List<IRecordValidator> validators)
        {
            this.validators = validators;
        }

        public void ValidateParametrs(RecordData recordData)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParametrs(recordData);
            }
        }
    }
}
