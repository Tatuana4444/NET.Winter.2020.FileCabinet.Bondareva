using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService service;

        public PurgeCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "purge")
            {
                this.Purge(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Purge(string parameters)
        {
            int purged = this.service.Purge();
            Console.WriteLine($"Data file processing is completed: {purged} of " +
                $"{this.service.GetStat().Item1 + purged} records were purged.");
        }
    }
}
