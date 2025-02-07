﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for command export.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public ExportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Export request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "EXPORT")
            {
                this.Export(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Export(string parameters)
        {
            string[] param = parameters.Split(' ');
            if (string.Equals(Path.GetExtension(param[1])[1..], param[0], StringComparison.InvariantCultureIgnoreCase))
            {
                if (File.Exists(param[1]))
                {
                    string answer = string.Empty;
                    bool isFind = false;
                    while (!isFind)
                    {
                        Console.Write($"File is exist - rewrite {param[1]}? [Y/n] ");
                        answer = Console.ReadLine();
                        if (string.Equals(answer, "Y", StringComparison.InvariantCultureIgnoreCase)
                            || string.Equals(answer, "N", StringComparison.InvariantCultureIgnoreCase))
                        {
                            isFind = true;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect answer. Please try again.");
                        }
                    }

                    if (string.Equals(answer, "N", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("Incorrect file extension.");
                return;
            }

            try
            {
                StreamWriter writer = new StreamWriter(param[1]);
                var snapshot = this.Service.MakeSnapshot();
                if (string.Equals(param[0], "csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    snapshot.SaveToCsv(writer);
                }
                else
                {
                    if (string.Equals(param[0], "xml", StringComparison.InvariantCultureIgnoreCase))
                    {
                        snapshot.SaveToXml(writer);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect file structure.");
                        writer.Close();
                        return;
                    }
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
    }
}
