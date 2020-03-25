using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///  Handler for command find.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public FindCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Find request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
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
                        filtedList = this.service.FindByFirstName(param[1].Substring(1, param[1].Length - 2));
                        break;
                    }

                case "LASTNAME":
                    {
                        filtedList = this.service.FindByLastName(param[1].Substring(1, param[1].Length - 2));
                        break;
                    }

                case "DATEOFBIRTH":
                    {
                        DateTimeStyles styles = DateTimeStyles.None;
                        if (DateTime.TryParse(param[1].Substring(1, param[1].Length - 2), englishUS, styles, out DateTime dateOfBirth))
                        {
                            filtedList = this.service.FindByDateOfBirth(dateOfBirth);
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
