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

            if (commandRequest.Command == "INSERT")
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
            culture.DateTimeFormat.ShortDatePattern = "MMM/dd/yyyy";
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

        private void Insert(string parameters)
        {
            string[] parametersName = { "id", "firstName", "lastname", "dateofbirth", "gender", "passportid", "salary" };
            bool[] isHere = new bool[7];
            string[] data = parameters.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length != 15 || data[7].Trim() != "values")
            {
                throw new ArgumentException("Uncorrect format of insert.");
            }

            for (int i = 8; i < 15; i++)
            {
                string temp = data[i].Trim();
                data[i] = temp[1..^1];
            }

            int id = default;
            string firstName = string.Empty, lastName = string.Empty;
            DateTime dateOfBirth = default;
            char gender = default;
            short passportId = default;
            decimal salary = default;
            for (int i = 0; i < 7; i++)
            {
                switch (data[i].Trim().ToUpperInvariant())
                {
                    case "ID":
                        isHere[0] = true;
                        if (!int.TryParse(data[i + 8], out id) || id <= 0)
                        {
                            throw new ArgumentException("Error, id should be integer and more than 0.");
                        }

                        break;
                    case "FIRSTNAME":
                        isHere[1] = true;
                        firstName = data[i + 8];
                        break;
                    case "LASTNAME":
                        isHere[2] = true;
                        lastName = data[i + 8];
                        break;
                    case "DATEOFBIRTH":
                        isHere[3] = true;
                        var concertingDate = DateConverter(data[i + 8]);
                        dateOfBirth = concertingDate.Item3;
                        break;
                    case "GENDER":
                        isHere[4] = true;
                        var convertingGender = CharConverter(data[i + 8]);
                        gender = convertingGender.Item3;
                        break;
                    case "PASSPORTID":
                        isHere[5] = true;
                        var convertingPassportId = ShortConverter(data[i + 8]);
                        passportId = convertingPassportId.Item3;
                        break;
                    case "SALARY":
                        isHere[6] = true;
                        var convertingSalaty = DecimalConverter(data[i + 8]);
                        salary = convertingSalaty.Item3;
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
