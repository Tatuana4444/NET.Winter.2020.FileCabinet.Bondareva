using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Validator.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// DefaultValidator.
        /// </summary>
        /// <param name="recordData">User's data.</param>
        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            new DefaultFirstNameValidator().ValidateParametrs(recordData);
            new DefaultLastNameValidator().ValidateParametrs(recordData);
            new DefaultDateOfBirthValidator().ValidateParametrs(recordData);
            new DefaultGenderValidator().ValidateParametrs(recordData);
            new DefaultPassportIdValidator().ValidateParametrs(recordData);
            new DefaultSalaryValidator().ValidateParametrs(recordData);
        }
    }
}
