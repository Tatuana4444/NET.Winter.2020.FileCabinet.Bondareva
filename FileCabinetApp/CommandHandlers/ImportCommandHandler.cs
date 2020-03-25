using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///  Handler for command import.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public ImportCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Import request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "import")
            {
                this.Import(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Import(string parameters)
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
                        this.service.Restore(snapshot);
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
                        this.service.Restore(snapshot);
                        Console.WriteLine($"{count} records were imported from {param[1]}");
                    }
                    else
                    {
                        Console.WriteLine($"Import error: file {param[1]} is not exist.");
                    }
                }
            }
        }
    }
}
