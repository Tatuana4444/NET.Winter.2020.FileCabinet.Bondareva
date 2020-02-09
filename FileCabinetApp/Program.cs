using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Tatyana Bondareva";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;

        private static FileCabinetService fileCabinetService = new FileCabinetService();

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

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
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

        private static void Create(string parameters)
        {
            string firstName;
            do
            {
                Console.Write("First name: ");
                firstName = Console.ReadLine();
                if (firstName.Length >= 2 && firstName.Length <= 60
                    && firstName.Trim().Length != 0)
                {
                    break;
                }
                else
                {
                    if (firstName.Length < 2 || firstName.Length > 60)
                    {
                        Console.WriteLine("Error, Length of first name can't be less than 2 and more than 60. Try again, please");
                    }

                    if (firstName.Trim().Length == 0)
                    {
                        Console.WriteLine("Error, First name can't contain only spaces. Try again, please");
                    }
                }
            }
            while (true);

            string lastName;
            do
            {
                Console.Write("Last name: ");
                lastName = Console.ReadLine();
                if (lastName.Length >= 2 && lastName.Length <= 60
                    && lastName.Trim().Length != 0)
                {
                    break;
                }
                else
                {
                    if (lastName.Length < 2 || lastName.Length > 60)
                    {
                        Console.WriteLine("Error, Length of last name can't be less than 2 and more than 60. Try again, please");
                    }

                    if (lastName.Trim().Length == 0)
                    {
                        Console.WriteLine("Error, Last name can't contain only spaces. Try again, please");
                    }
                }
            }
            while (true);
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeStyles styles = DateTimeStyles.None;
            DateTime dateOfBirth;
            do
            {
                Console.Write("Date of birth: ");
                if (DateTime.TryParse(Console.ReadLine(), culture, styles, out dateOfBirth))
                {
                    if ((DateTime.Compare(new DateTime(1950, 1, 1), dateOfBirth) > 0)
                        || (DateTime.Compare(DateTime.Now, dateOfBirth) < 0))
                    {
                        Console.WriteLine("Error, Date of Birth can't be less than 01-Jan-1950 and more than today. Try again, please");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Error, Date of birth should be in forma 'month/day/year'. Try again, please");
                }
            }
            while (true);
            char gender;
            do
            {
                Console.Write("Gender: ");
                if (char.TryParse(Console.ReadLine(), out gender))
                {
                    if (gender != 'W' && gender != 'M')
                    {
                        Console.WriteLine("Error, gender should be  \"M\" or \"W\". Try again, please");
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
            int index = Program.fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, gender, passportId, salary);
            Console.WriteLine($"Record #{index} is created.");
        }

        private static void List(string parameters)
        {
            FileCabinetRecord[] fileCabinetRecord = Program.fileCabinetService.GetRecords();
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

            string firstName;
            do
            {
                Console.Write("First name: ");
                firstName = Console.ReadLine();
                if (firstName.Length > 2 && firstName.Length < 60
                    && firstName.Trim().Length != 0)
                {
                    break;
                }
                else
                {
                    if (firstName.Length < 2 || firstName.Length > 60)
                    {
                        Console.WriteLine("Error, Length of first name can't be less than 2 and more than 60. Try again, please");
                    }

                    if (firstName.Trim().Length == 0)
                    {
                        Console.WriteLine("Error, First name can't contain only spaces. Try again, please");
                    }
                }
            }
            while (true);

            string lastName;
            do
            {
                Console.Write("Last name: ");
                lastName = Console.ReadLine();
                if (lastName.Length > 2 && lastName.Length < 60
                    && lastName.Trim().Length != 0)
                {
                    break;
                }
                else
                {
                    if (lastName.Length < 2 || lastName.Length > 60)
                    {
                        Console.WriteLine("Error, Length of last name can't be less than 2 and more than 60. Try again, please");
                    }

                    if (lastName.Trim().Length == 0)
                    {
                        Console.WriteLine("Error, Last name can't contain only spaces. Try again, please");
                    }
                }
            }
            while (true);
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeStyles styles = DateTimeStyles.None;
            DateTime dateOfBirth;
            do
            {
                Console.Write("Date of birth: ");
                if (DateTime.TryParse(Console.ReadLine(), culture, styles, out dateOfBirth))
                {
                    if ((DateTime.Compare(new DateTime(1950, 1, 1), dateOfBirth) > 0)
                        || (DateTime.Compare(DateTime.Now, dateOfBirth) < 0))
                    {
                        Console.WriteLine("Error, Date of Birth can't be less than 01-Jan-1950 and more than today. Try again, please");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Error, Date of birth should be in forma 'month/day/year'. Try again, please");
                }
            }
            while (true);
            char gender;
            do
            {
                Console.Write("Gender: ");
                if (char.TryParse(Console.ReadLine(), out gender))
                {
                    if (gender != 'W' && gender != 'M')
                    {
                        Console.WriteLine("Error, gender should be  \"M\" or \"W\". Try again, please");
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
            fileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth, gender, passportId, salary);
            Console.WriteLine($"Record #{id} is updated.");
        }

        private static void Find(string parameters)
        {
            CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");
            FileCabinetRecord[] filtedList = Array.Empty<FileCabinetRecord>();
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