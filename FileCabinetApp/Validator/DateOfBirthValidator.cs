using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for date of Birth.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;

        private readonly DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Min date.</param>
        /// <param name="to">Max date.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Check date of birth from date.
        /// </summary>
        /// <param name="recordData">Data.</param>
        /// <exception cref="ArgumentNullException">Throw when recordData is null.</exception>
        /// <exception cref="ArgumentException">Throw when DateOfBirth more than to or less than from.</exception>
        public void ValidateParameters(RecordData recordData)
        {
            if (recordData is null)
            {
                throw new ArgumentNullException(nameof(recordData), "RecordData name can't be null");
            }

            if ((DateTime.Compare(this.from, recordData.DateOfBirth) > 0)
                || (DateTime.Compare(this.to, recordData.DateOfBirth) < 0))
            {
                throw new ArgumentException("Date of Birth can't be less than 01-Jan-1900 and more than today", nameof(recordData));
            }
        }
    }
}
