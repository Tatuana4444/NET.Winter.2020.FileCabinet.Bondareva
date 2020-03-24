using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public interface ICommandHandler
    {
        public void SetNext(ICommandHandler handler);

        public void Handle(AppCommandRequest commandRequest);
    }
}
