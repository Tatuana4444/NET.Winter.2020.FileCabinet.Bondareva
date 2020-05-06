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
        private static readonly IConfiguration Config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("validation-rules.json")
             .Build();

        private readonly Dictionary<string, IRecordValidator> validators = new Dictionary<string, IRecordValidator>();

        /// <summary>
        /// Set validator for first name.
        /// </summary>
        /// <param name="min">Min length of first name.</param>
        /// <param name="max">Max length of first name.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add("FIRSTNAME", new FirstNameValidator(min, max));
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
            this.validators.Add("LASTNAME", new LastNameValidator(min, max));
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
            this.validators.Add("DATEOFBIRTH", new DateOfBirthValidator(from, to));
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
            this.validators.Add("GENDER", new GenderValidator(manSymbol, womanSymbol));
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
            this.validators.Add("PASSPORTID", new PassportIdValidator(min, max));
            return this;
        }

        /// <summary>
        /// Set validator for Salary.
        /// </summary>
        /// <param name="min">Min salary.</param>
        /// <returns>Current builder.</returns>
        public ValidatorBuilder ValidateSalary(decimal min)
        {
            this.validators.Add("SALARY", new SalaryValidator(min));
            return this;
        }

        /// <summary>
        /// Create Validator bu this builder.
        /// </summary>
        /// <returns>Validator.</returns>
        public IValidator Create()
        {
            return new CompositeValidator(this.validators);
        }

        /// <summary>
        /// Create Default Validator.
        /// </summary>
        /// <returns>Default Validator.</returns>
        public IValidator CreateDefault()
        {
            var defaultRules = Config.GetSection("default")
                .Get<ValidationRules>();
            return this.Create(defaultRules);
        }

        /// <summary>
        /// Create Custom Validator.
        /// </summary>
        /// <returns>Custom Validator.</returns>
        public IValidator CreateCustom()
        {
            var customRules = Config.GetSection("custom")
                .Get<ValidationRules>();
            return this.Create(customRules);
        }

        private IValidator Create(ValidationRules rules)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
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
