using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///  Handler for command stat.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public StatCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Stat request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "STAT")
            {
                if (commandRequest.Parameters.Length == 0)
                {
                    this.Stat();
                }
                else
                {
                    throw new ArgumentException("Incorrect parameter.");
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Stat()
        {
            var recordsCount = this.Service.GetStat();
            Console.WriteLine($"{recordsCount.Item1} record(s) and {recordsCount.Item2} deleted record(s).");
        }
    }
}
