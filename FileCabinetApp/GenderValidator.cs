using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class GenderValidator : IRecordValidator
    {
        private char manSymbol;

        private char womanSymbol;

        public GenderValidator(char manSymbol, char womanSymbol)
        {
            this.manSymbol = manSymbol;
            this.womanSymbol = womanSymbol;
        }

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
