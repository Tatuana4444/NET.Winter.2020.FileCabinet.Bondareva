using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class PassportIdValidator: IRecordValidator
    {
        private short minValue;

        private short maxValue;

        public PassportIdValidator(short minValue, short maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public void ValidateParametrs(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (recordData.PassportId < this.minValue || recordData.PassportId > this.maxValue)
            {
                throw new ArgumentException($"Passport Id can't be less than {this.minValue} and more than {this.maxValue}", nameof(recordData));
            }
        }
    }
}
