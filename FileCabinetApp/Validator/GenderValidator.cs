using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for gender.
    /// </summary>
    public class GenderValidator : IRecordValidator
    {
        private readonly char manSymbol;

        private readonly char womanSymbol;

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
        /// <exception cref="ArgumentNullException">Throw when recordData is null.</exception>
        /// <exception cref="ArgumentException">Throw when Gender isn't manSymbol or womanSymbol.</exception>
        public void ValidateParameters(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if (
                !string.Equals(
                    recordData.Gender.ToString(CultureInfo.InvariantCulture),
                    this.womanSymbol.ToString(CultureInfo.InvariantCulture),
                    StringComparison.InvariantCultureIgnoreCase)
                && !string.Equals(
                    recordData.Gender.ToString(CultureInfo.InvariantCulture),
                    this.manSymbol.ToString(CultureInfo.InvariantCulture),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException($"Gender should be {this.womanSymbol} or {this.manSymbol}", nameof(recordData));
            }
        }
    }
}
