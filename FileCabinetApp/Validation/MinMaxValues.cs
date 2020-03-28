using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Class that contains rules type that have min and max rules.
    /// </summary>
    /// <typeparam name="T">Type of values.</typeparam>
    public class MinMaxValues<T>
    {
        /// <summary>
        /// Gets or sets min value.
        /// </summary>
        /// <value>
        /// from date rules.
        /// </value>
        [JsonPropertyName("min")]
        public T Min { get; set; }

        /// <summary>
        /// Gets or sets max value.
        /// </summary>
        /// <value>
        /// from date rules.
        /// </value>
        [JsonPropertyName("max")]
        public T Max { get; set; }
    }
}
