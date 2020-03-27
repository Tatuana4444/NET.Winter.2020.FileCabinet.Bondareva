﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for passport id.
    /// </summary>
    public class PassportIdValidator : IRecordValidator
    {
        private short minValue;

        private short maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PassportIdValidator"/> class.
        /// </summary>
        /// <param name="minValue">Min value for passport id.</param>
        /// <param name="maxValue">Max value for passport id.</param>
        public PassportIdValidator(short minValue, short maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <summary>
        /// Check passport id from data.
        /// </summary>
        /// <param name="recordData">Data.</param>
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
