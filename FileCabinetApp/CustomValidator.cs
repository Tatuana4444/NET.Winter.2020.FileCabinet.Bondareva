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

            new CustomFirstNameValidator().ValidateParametrs(recordData);
            new CustomLastNameValidator().ValidateParametrs(recordData);
            new CustomDateOfBirthValidator().ValidateParametrs(recordData);
            new CustomGenderValidator().ValidateParametrs(recordData);
            new CustomPassportIdValidator().ValidateParametrs(recordData);
            new CustomSalaryValidator().ValidateParametrs(recordData);
        }
    }
}
