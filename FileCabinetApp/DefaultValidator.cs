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

            new FirstNameValidator(2, 60).ValidateParametrs(recordData);
            new LastNameValidator(2, 60).ValidateParametrs(recordData);
            new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now).ValidateParametrs(recordData);
            new GenderValidator('M', 'W').ValidateParametrs(recordData);
            new PassportIdValidator(1000, 9999).ValidateParametrs(recordData);
            new SalaryValidator(375).ValidateParametrs(recordData);
        }
    }
}
