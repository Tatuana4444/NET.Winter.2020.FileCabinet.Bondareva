using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Class that contains rules for gender.
    /// </summary>
    public class ManWoman
    {
        /// <summary>
        /// Gets or sets from symbol for man.
        /// </summary>
        /// <value>
        /// Symbol for man.
        /// </value>
        [JsonPropertyName("man")]
        public char Man { get; set; }

        /// <summary>
        /// Gets or sets symbol for woman.
        /// </summary>
        /// <value>
        /// Symbol for woman.
        /// </value>
        [JsonPropertyName("woman")]
        public char Woman { get; set; }
    }
}
