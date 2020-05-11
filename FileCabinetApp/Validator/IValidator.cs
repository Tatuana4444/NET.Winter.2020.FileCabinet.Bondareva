namespace FileCabinetApp
{
    /// <summary>
    /// Interface for validator.
    /// </summary>
    public interface IValidator : IRecordValidator
    {
        /// <summary>
        /// Validate data by parameter.
        /// </summary>
        /// <param name="validator">Name of Validator.</param>
        /// <param name="recordData">User's data.</param>
        public void ValidateParameter(string validator, RecordData recordData);
    }
}
