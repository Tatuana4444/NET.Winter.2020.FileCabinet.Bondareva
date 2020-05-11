using System;

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
        /// <param name="command">Current command.</param>
        /// <param name="parameters">Command parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown when command or parameters is null.</exception>
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
