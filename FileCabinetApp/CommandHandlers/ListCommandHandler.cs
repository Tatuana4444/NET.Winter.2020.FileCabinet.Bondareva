using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class ListCommandHandler : CommandHandlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "list")
            {
                this.List(commandRequest.Parameters);
            }
            else
            {

                base.Handle(commandRequest);
            }
        }

        private void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> fileCabinetRecord = Program.fileCabinetService.GetRecords();
            CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeFormatInfo dtfi = englishUS.DateTimeFormat;
            dtfi.ShortDatePattern = "yyyy-MMM-dd";
            foreach (FileCabinetRecord record in fileCabinetRecord)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("d", englishUS)}," +
                    $" {record.Gender}, {record.PassportId}, {record.Salary}");
            }
        }
    }
}
