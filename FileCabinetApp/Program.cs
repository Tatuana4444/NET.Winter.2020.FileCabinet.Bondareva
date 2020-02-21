using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class that gets comands and information from user and give it to FileCabinetService.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Tatyana Bondareva";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string DefaultValidationMessage = "Using default validation rules.";
        private const string CustomValidationMessage = "Using custom validation rules.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;

        private static IFileCabinetService fileCabinetService = new FileCabinetService(new DefaultValidator());

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "create", "creates new record", "The 'create' command creates new record." },
            new string[] { "edit", "edits record by id", "The 'edit' command edits record by id." },
            new string[] { "list", "prints list of records", "The 'create' command prints list of records." },
            new string[] { "find", "finds records by creterion", "The 'find' command finds records by creterion." },
            new string[] { "stat", "prints statistics by records", "The 'stat' command prints statistics by records." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        /// <summary>
        /// Gets command and calls their methods.
        /// </summary>
        /// <param name="args">Arguments from console runs.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            if (args != null && args.Length > 0)
            {
                if (args.Length == 1)
                {
                    string[] param = args[0].Split('=');
                    if (param[0] == "--validation-rules")
                    {
                        SetValidationRules(param[1]);
                    }
                }

                if (args.Length == 2)
                {
                    if (args[0] == "-v")
                    {
                        SetValidationRules(args[1]);
                    }
                }
            }
            else
            {
                Console.WriteLine(DefaultValidationMessage);
            }

            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void SetValidationRules(string param)
        {
            CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");

            if (param.ToUpper(englishUS) == "CUSTOM")
            {
                fileCabinetService = new FileCabinetService(new CustomValidator());
                Console.WriteLine(CustomValidationMessage);
            }

            if (param.ToUpper(englishUS) == "DEFAULT")
            {
                Console.WriteLine(DefaultValidationMessage);
            }
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static string GetNameFromConsole(string name)
        {
            string value;
            do
            {
                Console.Write($"{name}: ");
                value = Console.ReadLine();
                if (value.Length >= 2 && value.Length <= 60
                    && value.Trim().Length != 0)
                {
                    break;
                }
                else
                {
                    if (value.Length < 2 || value.Length > 60)
                    {
                        Console.WriteLine($"Error, Length of {name} can't be less than 2 and more than 60. Try again, please");
                    }

                    if (value.Trim().Length == 0)
                    {
                        Console.WriteLine($"Error, {name} can't contain only spaces. Try again, please");
                    }
                }
            }
            while (true);
            return value;
        }

        private static DateTime GetDateFromConsole()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeStyles styles = DateTimeStyles.None;
            DateTime date;
            do
            {
                Console.Write("Date of birth: ");
                if (DateTime.TryParse(Console.ReadLine(), culture, styles, out date))
                {
                    if ((DateTime.Compare(new DateTime(1950, 1, 1), date) > 0)
                        || (DateTime.Compare(DateTime.Now, date) < 0))
                    {
                        Console.WriteLine($"Error, Date of birth can't be less than 01-Jan-1950 and more than today. Try again, please");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine($"Error, Date of birth should be in forma 'month/day/year'. Try again, please");
                }
            }
            while (true);
            return date;
        }

        private static char GetGenderFromConsole()
        {
            char gender;
            do
            {
                Console.Write("Gender: ");
                if (char.TryParse(Console.ReadLine(), out gender))
                {
                    if (gender != 'W' && gender != 'M')
                    {
                        Console.WriteLine($"Error, Gender should be  \"M\" or \"W\". Try again, please");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Error, Unvalued value. Try again, please");
                }
            }
            while (true);
            return gender;
        }

        private static short GetPassportIdFromConsole()
        {
            short passportId;
            do
            {
                Console.Write("Pasport Id: ");
                if (short.TryParse(Console.ReadLine(), out passportId))
                {
                    if (passportId < 1000 || passportId > 9999)
                    {
                        Console.WriteLine("Error, Passport Id can't be less than 1000 and more than 9999. Try again, please");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Error, passportId should be short integer. Try again, please");
                }
            }
            while (true);
            return passportId;
        }

        private static decimal GetSalaryFromConsole()
        {
            decimal salary;
            do
            {
                Console.Write("Salary: ");
                if (decimal.TryParse(Console.ReadLine(), out salary))
                {
                    if (salary < FileCabinetService.MinSalary)
                    {
                        Console.WriteLine($"Error, Salary can't be less than {FileCabinetService.MinSalary}. Try again, please");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Error, salary should be decimal. Try again, please");
                }
            }
            while (true);
            return salary;
        }

        private static void Create(string parameters)
        {
            string firstName = GetNameFromConsole("First Name");
            string lastName = GetNameFromConsole("Last Name");
            DateTime dateOfBirth = GetDateFromConsole();
            char gender = GetGenderFromConsole();
            short passportId = GetPassportIdFromConsole();
            decimal salary = GetSalaryFromConsole();
            RecordData recordData = new RecordData(firstName, lastName, dateOfBirth, gender, passportId, salary);
            int index = Program.fileCabinetService.CreateRecord(recordData);
            Console.WriteLine($"Record #{index} is created.");
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
            if (!int.TryParse(parameters, out int id) || id > fileCabinetService.GetStat())
            {
                Console.WriteLine($"#{id} record is not found.");
                return;
            }

            string firstName = GetNameFromConsole("First Name");
            string lastName = GetNameFromConsole("Last Name");
            DateTime dateOfBirth = GetDateFromConsole();
            char gender = GetGenderFromConsole();
            short passportId = GetPassportIdFromConsole();
            decimal salary = GetSalaryFromConsole();
            RecordData recordData = new RecordData(firstName, lastName, dateOfBirth, gender, passportId, salary);
            fileCabinetService.EditRecord(id, recordData);
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
                        filtedList = fileCabinetService.FindByFirstName(param[1].Substring(1, param[1].Length - 2));
                        break;
                    }

                case "LASTNAME":
                    {
                        filtedList = fileCabinetService.FindByLastName(param[1].Substring(1, param[1].Length - 2));
                        break;
                    }

                case "DATEOFBIRTH":
                    {
                        DateTimeStyles styles = DateTimeStyles.None;
                        if (DateTime.TryParse(param[1].Substring(1, param[1].Length - 2), englishUS, styles, out DateTime dateOfBirth))
                        {
                            filtedList = fileCabinetService.FindByDateOfBirth(dateOfBirth);
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
    }
}