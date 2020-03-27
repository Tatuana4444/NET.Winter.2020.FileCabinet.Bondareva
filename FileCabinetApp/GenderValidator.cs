using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for gender.
    /// </summary>
    public class GenderValidator : IRecordValidator
    {
        private char manSymbol;

        private char womanSymbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="manSymbol">Symbol for man.</param>
        /// <param name="womanSymbol">Symbol for woman.</param>
        public GenderValidator(char manSymbol, char womanSymbol)
        {
            this.manSymbol = manSymbol;
            this.womanSymbol = womanSymbol;
        }

        /// <summary>
        /// Check gender from data.
        /// </summary>
        /// <param name="recordData">Data.</param>
        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.Gender != this.womanSymbol && recordData.Gender != this.manSymbol)
            {
                throw new ArgumentException($"Gender should be {this.womanSymbol} or {this.manSymbol}", nameof(recordData));
            }
        }
    }
}
