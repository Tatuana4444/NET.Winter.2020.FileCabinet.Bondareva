using System;
using System.IO;

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

            if (commandRequest.Command == "IMPORT")
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
                if (string.Equals(param[0], "csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (File.Exists(param[1]))
                    {
                        if (string.Equals(Path.GetExtension(param[1])[1..], "csv", StringComparison.InvariantCultureIgnoreCase))
                        {
                            FileStream stream = new FileStream(param[1], FileMode.Open);
                            var snapshot = new FileCabinetServiceSnapshot();
                            int count = snapshot.LoadFromCsv(new StreamReader(stream));
                            this.Service.Restore(snapshot);
                            Console.WriteLine($"{count} records were imported from {param[1]}");
                            stream.Close();
                        }
                        else
                        {
                            Console.WriteLine("Incorrect file extension.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Import error: file {param[1]} is not exist.");
                    }

                    return;
                }

                if (string.Equals(param[0], "xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (File.Exists(param[1]))
                    {
                        if (string.Equals(Path.GetExtension(param[1])[1..], "xml", StringComparison.InvariantCultureIgnoreCase))
                        {
                            FileStream stream = new FileStream(param[1], FileMode.Open);
                            var snapshot = new FileCabinetServiceSnapshot();
                            using var streamReader = new StreamReader(stream);
                            int count = snapshot.LoadFromXml(streamReader);
                            this.Service.Restore(snapshot);
                            Console.WriteLine($"{count} records were imported from {param[1]}");
                            stream.Close();
                        }
                        else
                        {
                            Console.WriteLine("Incorrect file extension.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Import error: file {param[1]} is not exist.");
                    }

                    return;
                }

                Console.WriteLine("Incorrect file structure.");
            }
        }
    }
}
