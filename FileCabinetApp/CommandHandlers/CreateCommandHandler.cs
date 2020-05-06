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

            if (commandRequest.Command == "CREATE")
            {
                if (commandRequest.Parameters.Length == 0)
                {
                    this.Create();
                }
                else
                {
                    Console.WriteLine("Incorrect parameters.");
                }
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
            CultureInfo culture = CultureInfo.InvariantCulture;
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

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter)
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

                return value;
            }
            while (true);
        }

        private void Create()
        {
            Console.Write("First name: ");
            var firstName = ReadInput(StringConverter);

            Console.Write("Last name: ");
            var lastName = ReadInput(StringConverter);

            Console.Write("Date of birth: ");
            var dateOfBirth = ReadInput(DateConverter);

            Console.Write("Gender: ");
            var gender = ReadInput(CharConverter);

            Console.Write("Passport ID: ");
            var passportId = ReadInput(ShortConverter);

            Console.Write("Salary: ");
            var salary = ReadInput(DecimalConverter);

            RecordData recordData = new RecordData(firstName, lastName, dateOfBirth, gender, passportId, salary);
            int index = this.Service.CreateRecord(recordData);
            Console.WriteLine($"Record #{index} is created.");
        }
    }
}