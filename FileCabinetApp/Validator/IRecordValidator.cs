using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Inferface for validators.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Implements validator.
        /// </summary>
        /// <param name="recordData">User's data.</param>
        public void ValidateParameters(RecordData recordData);
    }
}
