using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Builder for Validators.
    /// </summary>
    public class ValidatorBuilder
    {
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Set validator for first name.
        /// </summary>
        /// <param name="min">Min length of first name.</param>
        /// <param name="max">Max length of first name.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Set validator for last name.
        /// </summary>
        /// <param name="min">Min length of last name.</param>
        /// <param name="max">Max length of last name.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Set validator for date of birth.
        /// </summary>
        /// <param name="from">Min date date of birth.</param>
        /// <param name="to">Max date of bith.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Set validator for gender.
        /// </summary>
        /// <param name="manSymbol">Symbol for man.</param>
        /// <param name="womanSymbol">Symbol for woman.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateGender(char manSymbol, char womanSymbol)
        {
            this.validators.Add(new GenderValidator(manSymbol, womanSymbol));
            return this;
        }

        /// <summary>
        /// Set validator for passport Id.
        /// </summary>
        /// <param name="min">Min value of passport Id.</param>
        /// <param name="max">Max value of passport Id.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidatePassportId(short min, short max)
        {
            this.validators.Add(new PassportIdValidator(min, max));
            return this;
        }

        /// <summary>
        /// Set validator for Salary.
        /// </summary>
        /// <param name="min">Min salary.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateSalary(decimal min)
        {
            this.validators.Add(new SalaryValidator(min));
            return this;
        }

        /// <summary>
        /// Create Validator bu this builder.
        /// </summary>
        /// <returns>Validator.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }

        /// <summary>
        /// Create Default Validator.
        /// </summary>
        /// <returns>Default Validator.</returns>
        public IRecordValidator CreateDefault()
        {
            this.ValidateFirstName(2, 60);
            this.ValidateLastName(2, 60);
            this.ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Now);
            this.ValidateGender('M', 'W');
            this.ValidatePassportId(1000, 9999);
            this.ValidateSalary(375);
            return this.Create();
        }

        /// <summary>
        /// Create Custom Validator.
        /// </summary>
        /// <returns>Custom Validator.</returns>
        public IRecordValidator CreateCustom()
        {
            this.ValidateFirstName(2, 100);
            this.ValidateLastName(2, 100);
            this.ValidateDateOfBirth(new DateTime(1900, 1, 1), DateTime.Now);
            this.ValidateGender('M', 'W');
            this.ValidatePassportId(0, short.MaxValue);
            this.ValidateSalary(0);
            return this.Create();
        }
    }
}
