using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///  Handler for command create.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public CreateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Create request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "create")
            {
                this.Create(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static Tuple<bool, string, string> StringConverter(string data)
        {
            return new Tuple<bool, string, string>(true, string.Empty, data);
        }

        private static Tuple<bool, string, DateTime> DateConverter(string data)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeStyles styles = DateTimeStyles.None;
            if (DateTime.TryParse(data, culture, styles, out DateTime date))
            {
                return new Tuple<bool, string, DateTime>(true, string.Empty, date);
            }
            else
            {
                return new Tuple<bool, string, DateTime>(false, "Error, Date of birth should be in forma 'month/day/year'. Try again, please", date);
            }
        }

        private static Tuple<bool, string, char> CharConverter(string data)
        {
            if (char.TryParse(data, out char gender))
            {
                return new Tuple<bool, string, char>(true, string.Empty, gender);
            }
            else
            {
                return new Tuple<bool, string, char>(false, "Error, Unvalued value. Try again, please", gender);
            }
        }

        private static Tuple<bool, string, short> ShortConverter(string data)
        {
            if (short.TryParse(data, out short passportId))
            {
                return new Tuple<bool, string, short>(true, string.Empty, passportId);
            }
            else
            {
                return new Tuple<bool, string, short>(false, "Error, passportId should be short integer. Try again, please", passportId);
            }
        }

        private static Tuple<bool, string, decimal> DecimalConverter(string data)
        {
            if (decimal.TryParse(data, out decimal salary))
            {
                return new Tuple<bool, string, decimal>(true, string.Empty, salary);
            }
            else
            {
                return new Tuple<bool, string, decimal>(false, "Error, salary should be decimal. Try again, please", salary);
            }
        }

        private static Tuple<bool, string> FirstNameValidatorDefault(string firstName)
        {
            if (firstName.Length >= 2 && firstName.Length <= 60
                    && firstName.Trim().Length != 0)
            {
                return new Tuple<bool, string>(true, string.Empty);
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                return new Tuple<bool, string>(false, $"Error, Length of {nameof(firstName)} can't be less than 2 and more than 60. Try again, please");
            }

            return new Tuple<bool, string>(false, $"Error, {nameof(firstName)} can't contain only spaces. Try again, please");
        }

        private static Tuple<bool, string> LastNameValidatorDefault(string lastName)
        {
            if (lastName.Length >= 2 && lastName.Length <= 60
                    && lastName.Trim().Length != 0)
            {
                return new Tuple<bool, string>(true, string.Empty);
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                return new Tuple<bool, string>(false, $"Error, Length of {nameof(lastName)} can't be less than 2 and more than 60. Try again, please");
            }

            return new Tuple<bool, string>(false, $"Error, {nameof(lastName)} can't contain only spaces. Try again, please");
        }

        private static Tuple<bool, string> DateOfBirthValidatorDefault(DateTime dateOfBirth)
        {
            if ((DateTime.Compare(new DateTime(1950, 1, 1), dateOfBirth) > 0)
                        || (DateTime.Compare(DateTime.Now, dateOfBirth) < 0))
            {
                return new Tuple<bool, string>(false, "Error, Date of birth can't be less than 01-Jan-1950 and more than today. Try again, please");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> GenderValidator(char gender)
        {
            if (gender != 'W' && gender != 'M')
            {
                return new Tuple<bool, string>(false, "Error, Gender should be  \"M\" or \"W\". Try again, please");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> PassportIdValidatorDefault(short passportId)
        {
            if (passportId < 1000 || passportId > 9999)
            {
                return new Tuple<bool, string>(false, "Error, Passport Id can't be less than 1000 and more than 9999. Try again, please");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> SalaryValidatorDefault(decimal salary)
        {
            if (salary < FileCabinetMemoryService.MinSalary)
            {
                return new Tuple<bool, string>(false, $"Error, Salary can't be less than {FileCabinetMemoryService.MinSalary}. Try again, please");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> FirstNameValidatorCustom(string firstName)
        {
            if (firstName.Length >= 2 && firstName.Length <= 100
                    && firstName.Trim().Length != 0)
            {
                return new Tuple<bool, string>(true, string.Empty);
            }

            if (firstName.Length < 2 || firstName.Length > 100)
            {
                return new Tuple<bool, string>(false, $"Error, Length of {nameof(firstName)} can't be less than 2 and more than 100. Try again, please");
            }

            return new Tuple<bool, string>(false, $"Error, {nameof(firstName)} can't contain only spaces. Try again, please");
        }

        private static Tuple<bool, string> LastNameValidatorCustom(string lastName)
        {
            if (lastName.Length >= 2 && lastName.Length <= 100
                    && lastName.Trim().Length != 0)
            {
                return new Tuple<bool, string>(true, string.Empty);
            }

            if (lastName.Length < 2 || lastName.Length > 100)
            {
                return new Tuple<bool, string>(false, $"Error, Length of {nameof(lastName)} can't be less than 2 and more than 100. Try again, please");
            }

            return new Tuple<bool, string>(false, $"Error, {nameof(lastName)} can't contain only spaces. Try again, please");
        }

        private static Tuple<bool, string> DateOfBirthValidatorCustom(DateTime dateOfBirth)
        {
            if ((DateTime.Compare(new DateTime(1900, 1, 1), dateOfBirth) > 0)
                || (DateTime.Compare(DateTime.Now, dateOfBirth) < 0))
            {
                return new Tuple<bool, string>(false, "Error, Date of birth can't be less than 01-Jan-1900 and more than today. Try again, please");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> PassportIdValidatorCustom(short passportId)
        {
            if (passportId < 0)
            {
                return new Tuple<bool, string>(false, "Passport Id can't be less than 0. Try again, please");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> SalaryValidatorCustom(decimal salary)
        {
            if (salary < 0)
            {
                return new Tuple<bool, string>(false, $"Error, Salary can't be less than 0. Try again, please");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private void Create(string parameters)
        {
            Console.Write("First name: ");
            var firstName = Program.IsDefaulRule ? ReadInput(StringConverter, FirstNameValidatorDefault)
                : ReadInput(StringConverter, FirstNameValidatorCustom);

            Console.Write("Last name: ");
            var lastName = Program.IsDefaulRule ? ReadInput(StringConverter, LastNameValidatorDefault)
                : ReadInput(StringConverter, LastNameValidatorCustom);

            Console.Write("Date of birth: ");
            var dateOfBirth = Program.IsDefaulRule ? ReadInput(DateConverter, DateOfBirthValidatorDefault)
                : ReadInput(DateConverter, DateOfBirthValidatorCustom);

            Console.Write("Gender: ");
            var gender = ReadInput(CharConverter, GenderValidator);

            Console.Write("Passport ID: ");
            var passportId = Program.IsDefaulRule ? ReadInput(ShortConverter, PassportIdValidatorDefault)
                : ReadInput(ShortConverter, PassportIdValidatorCustom);

            Console.Write("Salary: ");
            var salary = Program.IsDefaulRule ? ReadInput(DecimalConverter, SalaryValidatorDefault)
                : ReadInput(DecimalConverter, SalaryValidatorCustom);

            RecordData recordData = new RecordData(firstName, lastName, dateOfBirth, gender, passportId, salary);
            int index = this.service.CreateRecord(recordData);
            Console.WriteLine($"Record #{index} is created.");
        }
    }
}
