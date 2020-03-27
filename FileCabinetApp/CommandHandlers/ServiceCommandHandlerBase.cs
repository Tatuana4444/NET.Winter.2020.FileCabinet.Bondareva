using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Base class for command handler whith service.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService service)
        {
            this.Service = service;
        }

        /// <summary>
        /// Gets or sets service.
        /// </summary>
        /// <value>
        /// Service.
        /// </value>
        protected IFileCabinetService Service { get; set; }
    }
}
