using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class CommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("purge", Purge),
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "create", "creates new record", "The 'create' command creates new record." },
            new string[] { "edit", "edits record by id", "The 'edit' command edits record by id." },
            new string[] { "remove", "removes record by id", "The 'remove' command removes record by id." },
            new string[] { "list", "prints list of records", "The 'create' command prints list of records." },
            new string[] { "find", "finds records by creterion", "The 'find' command finds records by creterion." },
            new string[] { "stat", "prints statistics by records", "The 'stat' command prints statistics by records." },
            new string[] { "export", "exports records", "The 'export' command expords records." },
            new string[] { "import", "imports records", "The 'import' command imports records." },
            new string[] { "purge", "purges records", "The 'purge' command purges records deleted records." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        public new void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(commandRequest.Command, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                commands[index].Item2(commandRequest.Parameters);
            }
            else
            {
                PrintMissedCommandInfo(commandRequest.Command);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHandler.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][CommandHandler.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHandler.CommandHelpIndex], helpMessage[CommandHandler.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount.Item1} record(s) and {recordsCount.Item2} deleted record(s).");
        }

        private static void Create(string parameters)
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
            int index = Program.fileCabinetService.CreateRecord(recordData);
            Console.WriteLine($"Record #{index} is created.");
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
            if (salary < DefaultValidator.MinSalary)
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

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> fileCabinetRecord = Program.fileCabinetService.GetRecords();
            CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeFormatInfo dtfi = englishUS.DateTimeFormat;
            dtfi.ShortDatePattern = "yyyy-MMM-dd";
            foreach (FileCabinetRecord record in fileCabinetRecord)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("d", englishUS)}," +
                    $" {record.Gender}, {record.PassportId}, {record.Salary}");
            }
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id) || id > Program.fileCabinetService.GetStat().Item1)
            {
                Console.WriteLine($"#{id} record is not found.");
                return;
            }

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
            Program.fileCabinetService.EditRecord(id, recordData);
            Console.WriteLine($"Record #{id} is updated.");
        }

        private static void Find(string parameters)
        {
            CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");
            ReadOnlyCollection<FileCabinetRecord> filtedList = new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
            DateTimeFormatInfo dtfi = englishUS.DateTimeFormat;
            dtfi.ShortDatePattern = "yyyy-MMM-dd";
            string[] param = parameters.Split(' ');
            switch (param[0].ToUpper(englishUS))
            {
                case "FIRSTNAME":
                    {
                        filtedList = Program.fileCabinetService.FindByFirstName(param[1].Substring(1, param[1].Length - 2));
                        break;
                    }

                case "LASTNAME":
                    {
                        filtedList = Program.fileCabinetService.FindByLastName(param[1].Substring(1, param[1].Length - 2));
                        break;
                    }

                case "DATEOFBIRTH":
                    {
                        DateTimeStyles styles = DateTimeStyles.None;
                        if (DateTime.TryParse(param[1].Substring(1, param[1].Length - 2), englishUS, styles, out DateTime dateOfBirth))
                        {
                            filtedList = Program.fileCabinetService.FindByDateOfBirth(dateOfBirth);
                        }

                        break;
                    }
            }

            foreach (FileCabinetRecord record in filtedList)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("d", englishUS)}," +
                $" {record.Gender}, {record.PassportId}, {record.Salary}");
            }
        }

        private static void Export(string parameters)
        {
            string[] param = parameters.Split(' ');
            if (File.Exists(param[1]))
            {
                char answer;
                do
                {
                    Console.Write($"File is exist - rewrite {param[1]}? [Y/n] ");
                    answer = (char)Console.Read();
                }
                while (answer != 'Y' && answer != 'n');
                if (answer == 'n')
                {
                    return;
                }
            }

            try
            {
                StreamWriter writer = new StreamWriter(param[1]);
                var snapshot = FileCabinetMemoryService.MakeSnapshot();
                if (param[0] == "csv")
                {
                    snapshot.SaveToCsv(writer);
                }

                if (param[0] == "xml")
                {
                    snapshot.SaveToXml(writer);
                }

                writer.Close();
                Console.WriteLine($"All records are exported to file {param[1]}.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"Export failed: can't open file {param[1]}.");
            }
            catch (IOException)
            {
                Console.WriteLine($"Export failed: can't open file {param[1]}.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Export failed: can't open file {param[1]}.");
            }
        }

        private static void Import(string parameters)
        {
            string[] param = parameters.Split(' ');
            if (param.Length == 2)
            {
                if (param[0] == "csv")
                {
                    if (File.Exists(param[1]))
                    {
                        FileStream stream = new FileStream(param[1], FileMode.Open);
                        var snapshot = new FileCabinetServiceSnapshot();
                        int count = snapshot.LoadFromCsv(new StreamReader(stream));
                        Program.fileCabinetService.Restore(snapshot);
                        Console.WriteLine($"{count} records were imported from {param[1]}");
                    }
                    else
                    {
                        Console.WriteLine($"Import error: file {param[1]} is not exist.");
                    }
                }

                if (param[0] == "xml")
                {
                    if (File.Exists(param[1]))
                    {
                        FileStream stream = new FileStream(param[1], FileMode.Open);
                        var snapshot = new FileCabinetServiceSnapshot();
                        using var streamReader = new StreamReader(stream);
                        int count = snapshot.LoadFromXml(streamReader);
                        Program.fileCabinetService.Restore(snapshot);
                        Console.WriteLine($"{count} records were imported from {param[1]}");
                    }
                    else
                    {
                        Console.WriteLine($"Import error: file {param[1]} is not exist.");
                    }
                }
            }
        }

        private static void Remove(string parameters)
        {
            if (int.TryParse(parameters, out int id))
            {
                if (Program.fileCabinetService.Remove(id))
                {
                    Console.WriteLine($"Record #{id} is removed.");
                }
                else
                {
                    Console.WriteLine($"Record #{id} doesn't exists.");
                }
            }
            else
            {
                Console.WriteLine("Invalide id.");
            }
        }

        private static void Purge(string parameters)
        {
            int purged = Program.fileCabinetService.Purge();
            Console.WriteLine($"Data file processing is completed: {purged} of " +
                $"{Program.fileCabinetService.GetStat().Item1 + purged} records were purged.");
        }
    }
}
