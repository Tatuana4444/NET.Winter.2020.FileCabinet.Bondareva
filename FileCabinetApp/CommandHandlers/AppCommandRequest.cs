using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Class, that contains command and parameters.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandRequest"/> class.
        /// </summary>
        /// <param name="command">Currend command.</param>
        /// <param name="parameters">Command's parameters.</param>
        public AppCommandRequest(string command, string parameters)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command), "Command can't be null.");
            }

            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters), "Parameters can't be null.");
            }

            this.Command = command.Trim().ToUpperInvariant();
            this.Parameters = parameters.Trim();
        }

        /// <summary>
        /// Gets Command.
        /// </summary>
        /// <value>
        /// Command.
        /// </value>
        public string Command { get; }

        /// <summary>
        /// Gets Parameters.
        /// </summary>
        /// <value>
        /// Parameters.
        /// </value>
        public string Parameters { get; }
    }
}
