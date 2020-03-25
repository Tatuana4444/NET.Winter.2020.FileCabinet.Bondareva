using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class StatCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService service;

        public StatCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "stat")
            {
                this.Stat(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Stat(string parameters)
        {
            var recordsCount = this.service.GetStat();
            Console.WriteLine($"{recordsCount.Item1} record(s) and {recordsCount.Item2} deleted record(s).");
        }
    }
}
