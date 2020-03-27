using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Default Validator.
    /// </summary>
    public class DefaultValidator : CompositeValidator
    {
        public DefaultValidator()
            : base(new List<IRecordValidator>
            {
                new FirstNameValidator(2, 60),
                new LastNameValidator(2, 60),
                new DateOfBirthValidator(new DateTime(1950, 1, 1), DateTime.Now),
                new GenderValidator('M', 'W'),
                new PassportIdValidator(1000, 9999),
                new SalaryValidator(375),
            })
        {
        }
    }
}
