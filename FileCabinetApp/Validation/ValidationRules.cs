using System.Text.Json.Serialization;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Class that contains validation rules.
    /// </summary>
    public class ValidationRules
    {
        /// <summary>
        /// Gets or sets firstName rules.
        /// </summary>
        /// <value>
        /// FirstName rules.
        /// </value>
        [JsonPropertyName("firstName")]
        public MinMaxValues<int> FirstName { get; set; }

        /// <summary>
        /// Gets or sets lastName rules.
        /// </summary>
        /// <value>
        /// lastName rules.
        /// </value>
        [JsonPropertyName("lastName")]
        public MinMaxValues<int> LastName { get; set; }

        /// <summary>
        /// Gets or sets DateOfBirth rules.
        /// </summary>
        /// <value>
        /// DateOfBirth rules.
        /// </value>
        [JsonPropertyName("dateOfBirth")]
        public DateFromToValues DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets Gender rules.
        /// </summary>
        /// <value>
        /// Gender rules.
        /// </value>
        [JsonPropertyName("gender")]
        public ManWoman Gender { get; set; }

        /// <summary>
        /// Gets or sets PasportId rules.
        /// </summary>
        /// <value>
        /// PassportId rules.
        /// </value>
        [JsonPropertyName("passportId")]
        public MinMaxValues<short> PassportId { get; set; }

        /// <summary>
        /// Gets or sets Salary rules.
        /// </summary>
        /// <value>
        /// Salary rules.
        /// </value>
        [JsonPropertyName("salary")]
        public MinSalary Salary { get; set; }
    }
}
