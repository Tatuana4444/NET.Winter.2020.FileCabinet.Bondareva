using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for command exit.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly Action<bool> exiting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="action">Delegate for existing.</param>
        public ExitCommandHandler(Action<bool> action)
        {
            this.exiting = action;
        }

        /// <summary>
        /// Exit request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "EXIT")
            {
                if (commandRequest.Parameters.Length == 0)
                {
                    this.Exit();
                }
                else
                {
                    throw new ArgumentException("Incorrect parameters.");
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Exit()
        {
            Console.WriteLine("Exiting an application...");
            this.exiting(false);
        }
    }
}
