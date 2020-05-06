using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for command delete.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">Current service.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Remove request handler.
        /// </summary>
        /// <param name="commandRequest">Request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), "CommandRequest can't be null.");
            }

            if (commandRequest.Command == "DELETE")
            {
                this.Delete(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Delete(string parameters)
        {
            List<int> deletedId = this.Service.Delete(parameters).ToList();
            if (deletedId.Count > 0)
            {
                Console.Write("Record ");
                for (int i = 0; i < deletedId.Count - 1; i++)
                {
                    Console.Write($"#{deletedId[i]}, ");
                }

                Console.WriteLine($"#{deletedId[deletedId.Count - 1]} is deleted.");
            }
            else
            {
                Console.WriteLine($"Record with such parameters doesn't exists.");
            }
        }
    }
}
