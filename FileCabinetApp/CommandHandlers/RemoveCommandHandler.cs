using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService service;

        public RemoveCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "remove")
            {
                this.Remove(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Remove(string parameters)
        {
            if (int.TryParse(parameters, out int id))
            {
                if (this.service.Remove(id))
                {
                    Console.WriteLine($"Record #{id} is removed.");
                }
                else
                {
                    Console.WriteLine($"Record #{id} doesn't exists.");
                }
            }
            else
            {
                Console.WriteLine("Invalide id.");
            }
        }
    }
}
