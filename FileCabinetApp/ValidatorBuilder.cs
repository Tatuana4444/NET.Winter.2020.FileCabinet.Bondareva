using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp.Validation;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Builder for Validators.
    /// </summary>
    public class ValidatorBuilder
    {
        private static IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("validation-rules.json")
             .Build();

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
            var defaultRules = config.GetSection("default")
                .Get<ValidationRules>();
            return this.Create(defaultRules);
        }

        /// <summary>
        /// Create Custom Validator.
        /// </summary>
        /// <returns>Custom Validator.</returns>
        public IRecordValidator CreateCustom()
        {
            var customRules = config.GetSection("custom")
                .Get<ValidationRules>();
            return this.Create(customRules);
        }

        private IRecordValidator Create(ValidationRules rules)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeStyles styles = DateTimeStyles.None;
            this.ValidateFirstName(rules.FirstName.Min, rules.FirstName.Max);
            this.ValidateLastName(rules.LastName.Min, rules.LastName.Max);
            DateTime.Parse(rules.DateOfBirth.From, culture, styles);
            this.ValidateDateOfBirth(
                DateTime.Parse(rules.DateOfBirth.From, culture, styles),
                DateTime.Parse(rules.DateOfBirth.To, culture, styles));
            this.ValidateGender(rules.Gender.Man, rules.Gender.Woman);
            this.ValidatePassportId(rules.PassportId.Min, rules.PassportId.Max);
            this.ValidateSalary(rules.Salary.Min);
            return this.Create();
        }
    }
}
