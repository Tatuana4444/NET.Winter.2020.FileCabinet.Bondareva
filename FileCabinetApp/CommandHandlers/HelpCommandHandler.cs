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

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "create", "creates new record", "The 'create' command creates new record.\nExample: create" },
            new string[] { "insert", "inserts record", "The 'insert' command inserts record.\nExample: insert (id, firstname, lastname, dateofbirth, gender, salary, passportId) values ('1', 'John', 'Doe', '5/18/1986', 'M', '1234', '1234')" },
            new string[] { "update", "updates records by parameters", "The 'update' command updates records by parameters.\nExample: update set firstname = 'John', lastname = 'Doe' , dateofbirth = '5/18/1986' where id = '1'" },
            new string[] { "delete", "deletes records by parameters", "The 'delete' command deletes records by parameters.\nExample: delete where id = '1'" },
            new string[] { "select", "selects records by parameters", "The 'select' command selects records by parameters.\nExample: select (id, firstname, lastname) where FirstName='Stan' and LastName='Smith'" },
            new string[] { "stat", "prints statistics by records", "The 'stat' command prints statistics by records.\nExample: stat" },
            new string[] { "export", "exports records", "The 'export' command expords records.\nExample: export csv fileName.csv" },
            new string[] { "import", "imports records", "The 'import' command imports records.\nExample: import csv fileName.csv" },
            new string[] { "purge", "purges records", "The 'purge' command purges records deleted records.\nExample: purge" },
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen.\nExample: help" },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application.\nExample: exit" },
        };

        /// <summary>
        /// Returns count of command.
        /// </summary>
        /// <returns>Count of command.</returns>
        public static int CommandsCount()
        {
            return HelpMessages.Length;
        }

        /// <summary>
        /// Get command name by id.
        /// </summary>
        /// <param name="i">Command's id.</param>
        /// <returns>Command name.</returns>
        public static string GetCommandName(int i)
        {
            return HelpMessages[i][0];
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
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[HelpCommandHandler.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][HelpCommandHandler.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[HelpCommandHandler.CommandHelpIndex], helpMessage[HelpCommandHandler.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
