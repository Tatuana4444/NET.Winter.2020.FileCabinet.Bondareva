using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Base class for command handler.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private const int SimilarCoefficient = 3;
        private ICommandHandler nextHandler;

        /// <summary>
        /// Base request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
        public virtual void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
            else
            {
                PrintMissedCommandInfo(commandRequest.Command);
                List<string> similarCommands = this.FindSimilarCommand(commandRequest.Command);
                this.PrintSimilarComands(similarCommands);
            }
        }

        /// <summary>
        /// Set next handler.
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
        }

        private static int LevenshteinDistance(string parameter, string command)
        {
            int m = parameter.Length, n = command.Length;
            int[][] levenshteinDistance = new int[m][];
            for (int i = 0; i < m; i++)
            {
                levenshteinDistance[i] = new int[n];
            }

            for (int i = 0; i < m; ++i)
            {
                levenshteinDistance[i][0] = i + 1;
            }

            for (int j = 0; j < n; ++j)
            {
                levenshteinDistance[0][j] = j + 1;
            }

            for (int j = 1; j < n; ++j)
            {
                for (int i = 1; i < m; ++i)
                {
                    if (parameter[i] == command[j])
                    {
                        levenshteinDistance[i][j] = levenshteinDistance[i - 1][j - 1];
                    }
                    else
                    {
                        levenshteinDistance[i][j] = Math.Min(levenshteinDistance[i - 1][j] + 1, Math.Min(levenshteinDistance[i][j - 1] + 1, levenshteinDistance[i - 1][j - 1] + 1));
                    }
                }
            }

            return levenshteinDistance[m - 1][n - 1];
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private void PrintSimilarComands(List<string> similarCommands)
        {
            if (similarCommands.Count > 1)
            {
                Console.WriteLine("The most similar commands are");
                for (int i = 0; i < similarCommands.Count; i++)
                {
                    Console.WriteLine(similarCommands[i]);
                }
            }
            else
            {
                if (similarCommands.Count == 1)
                {
                    Console.WriteLine("The most similar commands is");
                    Console.WriteLine(similarCommands[0]);
                }
                else
                {
                    Console.WriteLine("Similar commands hasn't found.");
                }
            }
        }

        private List<string> FindSimilarCommand(string parameter)
        {
            List<string> similarCommands = new List<string>();
            for (int i = 0; i < HelpCommandHandler.CommandsCount(); i++)
            {
                string command = HelpCommandHandler.GetCommandName(i);
                if (LevenshteinDistance(parameter, command) <= SimilarCoefficient)
                {
                    similarCommands.Add(command);
                }
            }

            return similarCommands;
        }
    }
}
