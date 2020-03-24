using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        public virtual void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
            else
            {
                PrintMissedCommandInfo(commandRequest.Command);
            }
        }

        public void SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}
