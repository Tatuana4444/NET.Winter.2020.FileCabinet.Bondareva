using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        public void Handle(AppCommandRequest commandRequest)
        {
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        public void SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;
        }
    }
}
