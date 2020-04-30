using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileCabinetApp.CommandHandlers;

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
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new ValidatorBuilder().CreateDefault());
        private static bool isRunning = true;

        /// <summary>
        /// Gets a value indicating whether defaulRule.
        /// </summary>
        /// <value>
        /// A value indicating whether defaulRule.
        /// </value>
        public static bool IsDefaulRule { get; private set; }

        /// <summary>
        /// Gets command and calls their methods.
        /// </summary>
        /// <param name="args">Arguments from console runs.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            string[] cmdParam = new string[] { "default", "file", "" };
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

                    if (args[i] == "use-stopwatch")
                    {
                        cmdParam[2] = "stopwatch";
                    }

                    if (args[i] == "use-logger")
                    {
                        cmdParam[2] = "logger";
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
            ICommandHandler commandHandler = CreateCommandHandlers(fileCabinetService);

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

        private static void Existing(bool isExist)
        {
            isRunning = isExist;
        }

        private static ICommandHandler CreateCommandHandlers(IFileCabinetService fileCabinetService)
        {
            ICommandHandler updateCommandHandler = new UpdateCommandHandler(fileCabinetService);
            ICommandHandler deleteCommandHandler = new DeleteCommandHandler(fileCabinetService);
            ICommandHandler selectCommand = new SelectCommandHandler(fileCabinetService, Program.PrinterByFilter);
            ICommandHandler statCommandHandler = new StatCommandHandler(fileCabinetService);
            ICommandHandler exportCommandHandler = new ExportCommandHandler();
            ICommandHandler importCommandHandler = new ImportCommandHandler(fileCabinetService);
            ICommandHandler purgeCommandHandler = new PurgeCommandHandler(fileCabinetService);
            ICommandHandler helpCommandHandler = new HelpCommandHandler();
            ICommandHandler exitCommandHandler = new ExitCommandHandler(Existing);
            ICommandHandler insertCommandHandler = new InsertCommandHandler(fileCabinetService);

            updateCommandHandler.SetNext(deleteCommandHandler);
            deleteCommandHandler.SetNext(selectCommand);
            selectCommand.SetNext(statCommandHandler);
            statCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(helpCommandHandler);
            helpCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(insertCommandHandler);

            return updateCommandHandler;
        }

        private static void PrinterByFilter(IEnumerable<FileCabinetRecord> records, string filter)
        {
            CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeFormatInfo dtfi = englishUS.DateTimeFormat;
            dtfi.ShortDatePattern = "yyyy-MMM-dd";
            string[] values = filter.Split(new string[] { ", ", "," }, StringSplitOptions.None);

            TWriter.WriteToTextSream(records, Console.Out, values, englishUS);
        }

        private static void SetValidationRules(string[] param)
        {
            CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");

            if (param[0].ToUpper(englishUS) == "CUSTOM")
            {
                if (param[1].ToUpper(englishUS) == "MEMORY")
                {
                    if (param[2] == "stopwatch")
                    {
                        var fileService = new FileCabinetMemoryService(new ValidatorBuilder().CreateCustom());
                        Program.fileCabinetService = new ServiceMeter(fileService);
                    }
                    else
                    {
                        if (param[2] == "logger")
                        {
                            var fileService = new FileCabinetMemoryService(new ValidatorBuilder().CreateCustom());
                            Program.fileCabinetService = new ServiceLogger(fileService);
                        }
                        else
                        {
                            Program.fileCabinetService = new FileCabinetMemoryService(new ValidatorBuilder().CreateCustom());
                        }
                    }
                }
                else
                {
                    File.Delete("cabinet-records.db");
                    FileStream stream = new FileStream("cabinet-records.db", FileMode.OpenOrCreate);
                    if (param[2] == "stopwatch")
                    {
                        var fileService = new FileCabinetFilesystemService(stream, new ValidatorBuilder().CreateCustom());
                        Program.fileCabinetService = new ServiceMeter(fileService);
                    }
                    else
                    {
                        if (param[2] == "logger")
                        {
                            var fileService = new FileCabinetFilesystemService(stream, new ValidatorBuilder().CreateCustom());
                            Program.fileCabinetService = new ServiceLogger(fileService);
                        }
                        else
                        {
                            Program.fileCabinetService = new FileCabinetFilesystemService(stream, new ValidatorBuilder().CreateCustom());
                        }
                    }
                }

                Program.IsDefaulRule = false;
                Console.WriteLine(CustomValidationMessage);
            }

            if (param[0].ToUpper(englishUS) == "DEFAULT")
            {
                if (param[1].ToUpper(englishUS) == "MEMORY")
                {
                    if (param[2] == "stopwatch")
                    {
                        var fileService = new FileCabinetMemoryService(new ValidatorBuilder().CreateDefault());
                        Program.fileCabinetService = new ServiceMeter(fileService);
                    }
                    else
                    {
                        if (param[2] == "logger")
                        {
                            var fileService = new FileCabinetMemoryService(new ValidatorBuilder().CreateDefault());
                            Program.fileCabinetService = new ServiceLogger(fileService);
                        }
                        else
                        {
                            Program.fileCabinetService = new FileCabinetMemoryService(new ValidatorBuilder().CreateDefault());
                        }
                    }
                }
                else
                {
                    File.Delete("cabinet-records.db");
                    FileStream stream = new FileStream("cabinet-records.db", FileMode.OpenOrCreate);
                    if (param[2] == "stopwatch")
                    {
                        var fileService = new FileCabinetFilesystemService(stream, new ValidatorBuilder().CreateDefault());
                        Program.fileCabinetService = new ServiceMeter(fileService);
                    }
                    else
                    {
                        if (param[2] == "logger")
                        {
                            var fileService = new FileCabinetFilesystemService(stream, new ValidatorBuilder().CreateDefault());
                            Program.fileCabinetService = new ServiceLogger(fileService);
                        }
                        else
                        {
                            Program.fileCabinetService = new FileCabinetFilesystemService(stream, new ValidatorBuilder().CreateDefault());
                        }
                    }
                }

                Program.IsDefaulRule = true;
                Console.WriteLine(DefaultValidationMessage);
            }
        }
    }
}