using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Custom Validator.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Implements cusstom validator.
        /// </summary>
        /// <param name="recordData">User's data.</param>
        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            new FirstNameValidator(2, 100).ValidateParametrs(recordData);
            new LastNameValidator(2, 100).ValidateParametrs(recordData);
            new DateOfBirthValidator(new DateTime(1900, 1, 1), DateTime.Now).ValidateParametrs(recordData);
            new GenderValidator('M', 'W').ValidateParametrs(recordData);
            new PassportIdValidator(0, short.MaxValue).ValidateParametrs(recordData);
            new SalaryValidator(0).ValidateParametrs(recordData);
        }
    }
}
