using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///  Handler for command insert.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public InsertCommandHandler(IFileCabinetService service)
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

            if (commandRequest.Command == "insert")
            {
                this.Insert(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
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

        private void Insert(string parameters)
        {
            string[] parametersName = { "id", "firstName", "lastname", "dateofbirth", "gender", "passportid", "salary" };
            bool[] isHere = new bool[7];
            string[] data = parameters.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length != 15 || data[7].Trim() != "values")
            {
                throw new FormatException("Uncorrect format of insert.");
            }

            for (int i = 8; i < 15; i++)
            {
                string temp = data[i].Trim();
                data[i] = temp[1..^1];
            }

            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            int id = default;
            string firstName = string.Empty, lastName = string.Empty;
            DateTime dateOfBirth = default;
            char gender = default;
            short passportId = default;
            decimal salary = default;
            for (int i = 0; i < 7; i++)
            {
                switch (data[i].Trim().ToLower(culture))
                {
                    case "id":
                        isHere[0] = true;
                        if (!int.TryParse(data[i + 8], out id) || id <= 0)
                        {
                            throw new ArgumentException("Error, id should be integer and more than 0.");
                        }

                        break;
                    case "firstname":
                        isHere[1] = true;
                        var valideFirstName = Program.IsDefaulRule ? FirstNameValidatorDefault(data[i + 8])
                                : FirstNameValidatorCustom(data[i + 8]);
                        if (valideFirstName.Item1)
                        {
                            firstName = data[i + 8];
                        }
                        else
                        {
                            throw new ArgumentException(valideFirstName.Item2, nameof(parameters));
                        }

                        break;
                    case "lastname":
                        isHere[2] = true;
                        var valideLastName = Program.IsDefaulRule ? LastNameValidatorDefault(data[i + 8])
                                : LastNameValidatorCustom(data[i + 8]);
                        if (valideLastName.Item1)
                        {
                            lastName = data[i + 8];
                        }
                        else
                        {
                            throw new ArgumentException(valideLastName.Item2, nameof(parameters));
                        }

                        break;
                    case "dateofbirth":
                        isHere[3] = true;
                        var concertingDate = DateConverter(data[i + 8]);
                        if (concertingDate.Item1)
                        {
                            var valideDate = Program.IsDefaulRule ? DateOfBirthValidatorDefault(concertingDate.Item3)
                                : DateOfBirthValidatorCustom(concertingDate.Item3);
                            if (valideDate.Item1)
                            {
                                dateOfBirth = concertingDate.Item3;
                            }
                            else
                            {
                                throw new ArgumentException(valideDate.Item2, nameof(parameters));
                            }
                        }
                        else
                        {
                            throw new FormatException(concertingDate.Item2);
                        }

                        break;
                    case "gender":
                        isHere[4] = true;
                        var convertingGender = CharConverter(data[i + 8]);
                        if (convertingGender.Item1)
                        {
                            var valideGender = GenderValidator(convertingGender.Item3);
                            if (valideGender.Item1)
                            {
                                gender = convertingGender.Item3;
                            }
                            else
                            {
                                throw new ArgumentException(valideGender.Item2, nameof(parameters));
                            }
                        }
                        else
                        {
                            throw new FormatException(convertingGender.Item2);
                        }

                        break;
                    case "passportid":
                        isHere[5] = true;
                        var convertingPassportId = ShortConverter(data[i + 8]);
                        if (convertingPassportId.Item1)
                        {
                            var validePassportId = Program.IsDefaulRule ? PassportIdValidatorDefault(convertingPassportId.Item3)
                                : PassportIdValidatorCustom(convertingPassportId.Item3);
                            if (validePassportId.Item1)
                            {
                                passportId = convertingPassportId.Item3;
                            }
                            else
                            {
                                throw new ArgumentException(validePassportId.Item2, nameof(parameters));
                            }
                        }
                        else
                        {
                            throw new FormatException(convertingPassportId.Item2);
                        }

                        break;
                    case "salary":
                        isHere[6] = true;
                        var convertingSalaty = DecimalConverter(data[i + 8]);
                        if (convertingSalaty.Item1)
                        {
                            var valideSalary = Program.IsDefaulRule ? SalaryValidatorDefault(convertingSalaty.Item3)
                                : SalaryValidatorCustom(convertingSalaty.Item3);
                            if (valideSalary.Item1)
                            {
                                salary = convertingSalaty.Item3;
                            }
                            else
                            {
                                throw new ArgumentException(valideSalary.Item2, nameof(parameters));
                            }
                        }
                        else
                        {
                            throw new FormatException(convertingSalaty.Item2);
                        }

                        break;
                }
            }

            for (int i = 0; i < 7; i++)
            {
                if (!isHere[i])
                {
                    throw new ArgumentException($"Missing parameter {parametersName[i]}.", nameof(parameters));
                }
            }

            RecordData recordData = new RecordData(id, firstName, lastName, dateOfBirth, gender, passportId, salary);
            int index = this.Service.CreateRecord(recordData);
            Console.WriteLine($"Record #{index} is inserted.");
        }
    }
}
