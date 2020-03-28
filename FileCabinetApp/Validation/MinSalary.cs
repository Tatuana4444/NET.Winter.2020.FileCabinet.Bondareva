using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Class that contains rules for salary.
    /// </summary>
    public class MinSalary
    {
        /// <summary>
        /// Gets or sets min salary.
        /// </summary>
        /// <value>
        /// from date rules.
        /// </value>
        [JsonPropertyName("min")]
        public decimal Min { get; set; }
    }
}
