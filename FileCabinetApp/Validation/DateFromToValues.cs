using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Class that contains rules for date.
    /// </summary>
    public class DateFromToValues
    {
        /// <summary>
        /// Gets or sets from date rules.
        /// </summary>
        /// <value>
        /// from date rules.
        /// </value>
        [JsonPropertyName("from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets to date rules.
        /// </summary>
        /// <value>
        /// to date rules.
        /// </value>
        [JsonPropertyName("to")]
        public string To { get; set; }
    }
}
