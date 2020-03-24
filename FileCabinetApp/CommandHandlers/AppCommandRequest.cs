using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class AppCommandRequest
    {
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }

        public string Command { get; }

        public string Parameters { get; }
    }
}
