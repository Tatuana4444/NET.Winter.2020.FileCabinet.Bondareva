using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///  Handler for command purge.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public PurgeCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Purge request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "purge")
            {
                if (commandRequest.Parameters.Length == 0)
                {
                    this.Purge();
                }
                else
                {
                    Console.WriteLine("Incorrect parameters.");
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Purge()
        {
            int purged = this.Service.Purge();
            Console.WriteLine($"Data file processing is completed: {purged} of " +
                $"{this.Service.GetStat().Item1 + purged} records were purged.");
        }
    }
}
