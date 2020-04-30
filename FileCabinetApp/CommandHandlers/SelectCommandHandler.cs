using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///  Handler for command list.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>, string> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        /// <param name="printer">Current printer.</param>
        public SelectCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>, string> printer)
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

            if (commandRequest.Command == "select")
            {
                this.Select(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Select(string parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            int whereIndex = parameters.IndexOf("where", StringComparison.Ordinal);
            if (whereIndex == -1)
            {
                throw new ArgumentException("Incorrect format", nameof(parameters));
            }

            string whereParams = parameters.Substring(whereIndex, parameters.Length - whereIndex);
            ReadOnlyCollection<FileCabinetRecord> fileCabinetRecords = this.Service.SelectRecords(whereParams);

            this.printer(fileCabinetRecords, parameters.Substring(0, whereIndex - 1));
        }
    }
}
