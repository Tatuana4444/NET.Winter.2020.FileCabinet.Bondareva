using FileCabinetApp.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Class that gets comands and information from user and give it to FileCabinetService.
    /// </summary>
    public static class Program
    {

        public static bool isRunning = true;
        public static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());

        private const string DeveloperName = "Tatyana Bondareva";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string DefaultValidationMessage = "Using default validation rules.";
        private const string CustomValidationMessage = "Using custom validation rules.";

        public static bool IsDefaulRule { get; private set; }

        /// <summary>
        /// Gets command and calls their methods.
        /// </summary>
        /// <param name="args">Arguments from console runs.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            string[] cmdParam = new string[] { "default", "file" };
            if (args != null && args.Length > 0)
            {
                int i = 0;
                while (i < args.Length)
                {
                    if (args[i] == "-v")
                    {
                        cmdParam[0] = args[++i];
                    }

                    if (args[i] == "-s")
                    {
                        cmdParam[1] = args[++i];
                    }

                    string[] param = args[i].Split('=');
                    if (param.Length == 2 && param[0] == "--validation-rules")
                    {
                        cmdParam[0] = param[1];
                    }

                    if (param.Length == 2 && param[0] == "--storage ")
                    {
                        cmdParam[0] = param[1];
                    }

                    i++;
                }
            }

            SetValidationRules(cmdParam);
            var commandHandler = CreateCommandHandlers();

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

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                commandHandler.Handle(new AppCommandRequest(command,  parameters));
            }
            while (isRunning);
        }

        private static CommandHandler CreateCommandHandlers()
        {
            var commandHandler = new CommandHandler();
            return commandHandler;
        }

        private static void SetValidationRules(string[] param)
        {
            CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");

            if (param[0].ToUpper(englishUS) == "CUSTOM")
            {
                if (param[1].ToUpper(englishUS) == "MEMORY")
                {
                    Program.fileCabinetService = new FileCabinetMemoryService(new CustomValidator());
                }
                else
                {
                    File.Delete("cabinet-records.db");
                    FileStream stream = new FileStream("cabinet-records.db", FileMode.OpenOrCreate);
                    Program.fileCabinetService = new FileCabinetFilesystemService(stream, new CustomValidator());
                }

                Program.IsDefaulRule = false;
                Console.WriteLine(CustomValidationMessage);
            }

            if (param[0].ToUpper(englishUS) == "DEFAULT")
            {
                if (param[1].ToUpper(englishUS) == "MEMORY")
                {
                    Program.fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
                }
                else
                {
                    File.Delete("cabinet-records.db");
                    FileStream stream = new FileStream("cabinet-records.db", FileMode.OpenOrCreate);
                    Program.fileCabinetService = new FileCabinetFilesystemService(stream, new DefaultValidator());
                }

                Program.IsDefaulRule = true;
                Console.WriteLine(DefaultValidationMessage);
            }
        }
    }
}