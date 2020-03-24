using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class FindCommandHandler : CommandHandlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "find")
            {
                this.Find(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Find(string parameters)
        {
            CultureInfo englishUS = CultureInfo.CreateSpecificCulture("en-US");
            ReadOnlyCollection<FileCabinetRecord> filtedList = new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
            DateTimeFormatInfo dtfi = englishUS.DateTimeFormat;
            dtfi.ShortDatePattern = "yyyy-MMM-dd";
            string[] param = parameters.Split(' ');
            switch (param[0].ToUpper(englishUS))
            {
                case "FIRSTNAME":
                    {
                        filtedList = Program.fileCabinetService.FindByFirstName(param[1].Substring(1, param[1].Length - 2));
                        break;
                    }

                case "LASTNAME":
                    {
                        filtedList = Program.fileCabinetService.FindByLastName(param[1].Substring(1, param[1].Length - 2));
                        break;
                    }

                case "DATEOFBIRTH":
                    {
                        DateTimeStyles styles = DateTimeStyles.None;
                        if (DateTime.TryParse(param[1].Substring(1, param[1].Length - 2), englishUS, styles, out DateTime dateOfBirth))
                        {
                            filtedList = Program.fileCabinetService.FindByDateOfBirth(dateOfBirth);
                        }

                        break;
                    }
            }

            foreach (FileCabinetRecord record in filtedList)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("d", englishUS)}," +
                $" {record.Gender}, {record.PassportId}, {record.Salary}");
            }
        }
    }
}
