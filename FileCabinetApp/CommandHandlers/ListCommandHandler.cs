using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///  Handler for command list.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        /// <param name="printer">Current printer.</param>
        public ListCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(service)
        {
            this.printer = printer;
        }

        /// <summary>
        /// List request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
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
            ReadOnlyCollection<FileCabinetRecord> fileCabinetRecord = this.Service.GetRecords();
            this.printer(fileCabinetRecord);
        }
    }
}
