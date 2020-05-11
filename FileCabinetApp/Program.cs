using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// Class that gets commands and information from user and give it to FileCabinetService.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Tatyana Bondareva";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string DefaultValidationMessage = "Using default validation rules.";
        private const string CustomValidationMessage = "Using custom validation rules.";
        private const string LoggerMessage = "Using Logger.";
        private const string StopwatchMessage = "Using Meter.";
        private const string FileMessage = "Using filesystem storage.";
        private const string MemoryMessage = "Using memory storage.";
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new ValidatorBuilder().CreateDefault());
        private static bool isRunning = true;

        /// <summary>
        /// Gets command and calls their methods.
        /// </summary>
        /// <param name="args">Arguments from console runs.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            string[] cmdParam = new string[] { "DEFAULT", "MEMORY", string.Empty, string.Empty };
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
                        cmdParam[2] = "STOPWATCH";
                    }

                    if (args[i] == "use-logger")
                    {
                        cmdParam[3] = "LOGGER";
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
                var inputs = Console.ReadLine()?.Trim().Split(' ', 2);
                const int commandIndex = 0;
                if (inputs != null)
                {
                    var command = inputs[commandIndex];

                    if (string.IsNullOrEmpty(command))
                    {
                        Console.WriteLine(Program.HintMessage);
                        continue;
                    }

                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    try
                    {
                        commandHandler.Handle(new AppCommandRequest(command, parameters));
                    }
                    catch (ArgumentNullException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            while (isRunning);
        }

        private static void Existing(bool isExist)
        {
            isRunning = isExist;
        }

        private static ICommandHandler CreateCommandHandlers(IFileCabinetService fileCabinetService)
        {
            ICommandHandler createCommandHandler = new CreateCommandHandler(fileCabinetService);
            ICommandHandler updateCommandHandler = new UpdateCommandHandler(fileCabinetService);
            ICommandHandler deleteCommandHandler = new DeleteCommandHandler(fileCabinetService);
            ICommandHandler selectCommand = new SelectCommandHandler(fileCabinetService, Program.PrinterByFilter);
            ICommandHandler statCommandHandler = new StatCommandHandler(fileCabinetService);
            ICommandHandler exportCommandHandler = new ExportCommandHandler(fileCabinetService);
            ICommandHandler importCommandHandler = new ImportCommandHandler(fileCabinetService);
            ICommandHandler purgeCommandHandler = new PurgeCommandHandler(fileCabinetService);
            ICommandHandler helpCommandHandler = new HelpCommandHandler();
            ICommandHandler exitCommandHandler = new ExitCommandHandler(Existing);
            ICommandHandler insertCommandHandler = new InsertCommandHandler(fileCabinetService);

            createCommandHandler.SetNext(updateCommandHandler);
            updateCommandHandler.SetNext(deleteCommandHandler);
            deleteCommandHandler.SetNext(selectCommand);
            selectCommand.SetNext(statCommandHandler);
            statCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(helpCommandHandler);
            helpCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(insertCommandHandler);

            return createCommandHandler;
        }

        private static void PrinterByFilter(IEnumerable<FileCabinetRecord> records, string filter)
        {
            string[] values = filter.Split(new string[] { ", ", "," }, StringSplitOptions.None);

            TWriter.WriteToTextStream(records, Console.Out, values);
        }

        private static void SetDecorators(string[] param, IFileCabinetService service)
        {
            if (string.Equals(param[2], "STOPWATCH", StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(param[3], "LOGGER", StringComparison.InvariantCultureIgnoreCase))
            {
                Program.fileCabinetService = new ServiceLogger(new ServiceMeter(service));
                Console.WriteLine(LoggerMessage);
                Console.WriteLine(StopwatchMessage);
            }
            else
            {
                if (string.Equals(param[3], "LOGGER", StringComparison.InvariantCultureIgnoreCase))
                {
                    Program.fileCabinetService = new ServiceLogger(service);
                    Console.WriteLine(LoggerMessage);
                }
                else
                {
                    if (string.Equals(param[2], "STOPWATCH", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Program.fileCabinetService = new ServiceMeter(service);
                        Console.WriteLine(StopwatchMessage);
                    }
                    else
                    {
                        Program.fileCabinetService = service;
                    }
                }
            }
        }

        private static void SetStorage(string[] param, IValidator validator)
        {
            if (string.Equals(param[1], "MEMORY", StringComparison.InvariantCultureIgnoreCase))
            {
                SetDecorators(param, new FileCabinetMemoryService(validator));
                Console.WriteLine(MemoryMessage);
            }
            else
            {
                FileStream stream = new FileStream("cabinet-records.db", FileMode.OpenOrCreate);
                SetDecorators(param, new FileCabinetFilesystemService(stream, validator));
                Console.WriteLine(FileMessage);
            }
        }

        private static void SetValidationRules(string[] param)
        {
            if (string.Equals(param[0], "CUSTOM", StringComparison.InvariantCultureIgnoreCase))
            {
                SetStorage(param, new ValidatorBuilder().CreateCustom());
                Console.WriteLine(CustomValidationMessage);
            }

            if (string.Equals(param[0], "DEFAULT", StringComparison.InvariantCultureIgnoreCase))
            {
                SetStorage(param, new ValidatorBuilder().CreateDefault());
                Console.WriteLine(DefaultValidationMessage);
            }
        }
    }
}