using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface for printer.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Print records in some formar.
        /// </summary>
        /// <param name="records">Records.</param>
        public void Print(IEnumerable<FileCabinetRecord> records);
    }
}
