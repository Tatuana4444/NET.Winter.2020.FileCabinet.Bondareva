using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Interface for handlers command.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Set next handler.
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void SetNext(ICommandHandler handler);

        /// <summary>
        /// Base request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
        public void Handle(AppCommandRequest commandRequest);
    }
}
