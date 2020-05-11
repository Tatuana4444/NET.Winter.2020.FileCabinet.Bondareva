namespace FileCabinetApp
{
    /// <summary>
    /// Interface for validators.
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
