using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for command help.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "create", "creates new record", "The 'create' command creates new record." },
            new string[] { "insert", "inserts record", "The 'insert' command inserts record." },
            new string[] { "update", "updates records by parameters", "The 'update' command updates records by parameters." },
            new string[] { "delete", "deletes records by parameters", "The 'delete' command deletes records by parameters." },
            new string[] { "select", "selects records by parameters", "The 'select' command selects records by parameters." },
            new string[] { "stat", "prints statistics by records", "The 'stat' command prints statistics by records." },
            new string[] { "export", "exports records", "The 'export' command expords records." },
            new string[] { "import", "imports records", "The 'import' command imports records." },
            new string[] { "purge", "purges records", "The 'purge' command purges records deleted records." },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        /// <summary>
        /// Returns count of command.
        /// </summary>
        /// <returns>Count of command.</returns>
        public static int CommandsCount()
        {
            return helpMessages.Length;
        }

        /// <summary>
        /// Get command name by id.
        /// </summary>
        /// <param name="i">Command's id.</param>
        /// <returns>Command name.</returns>
        public static string GetCommandName(int i)
        {
            return helpMessages[i][0];
        }

        /// <summary>
        /// Help request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "help")
            {
                this.PrintHelp(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[HelpCommandHandler.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][HelpCommandHandler.ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[HelpCommandHandler.CommandHelpIndex], helpMessage[HelpCommandHandler.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
