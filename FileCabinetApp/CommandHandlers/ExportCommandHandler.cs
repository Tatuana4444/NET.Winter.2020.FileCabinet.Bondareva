using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class ExportCommandHandler : CommandHandlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "export")
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
            if (File.Exists(param[1]))
            {
                char answer;
                do
                {
                    Console.Write($"File is exist - rewrite {param[1]}? [Y/n] ");
                    answer = (char)Console.Read();
                }
                while (answer != 'Y' && answer != 'n');
                if (answer == 'n')
                {
                    return;
                }
            }

            try
            {
                StreamWriter writer = new StreamWriter(param[1]);
                var snapshot = FileCabinetMemoryService.MakeSnapshot();
                if (param[0] == "csv")
                {
                    snapshot.SaveToCsv(writer);
                }

                if (param[0] == "xml")
                {
                    snapshot.SaveToXml(writer);
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
