using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Custom Validator.
    /// </summary>
    public class CustomValidator : CompositeValidator
    {
        public CustomValidator()
            : base(new List<IRecordValidator>
            {
                new FirstNameValidator(2, 100),
                new LastNameValidator(2, 100),
                new DateOfBirthValidator(new DateTime(1900, 1, 1), DateTime.Now),
                new GenderValidator('M', 'W'),
                new PassportIdValidator(0, short.MaxValue),
                new SalaryValidator(0),
            })
        {
        }
    }
}
