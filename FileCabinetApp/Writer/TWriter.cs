using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Writer for type T.
    /// </summary>
    public static class TWriter
    {
        /// <summary>
        /// Write IEnumerable T to text stream.
        /// </summary>
        /// <typeparam name="T">Type of records.</typeparam>
        /// <param name="records">Rcords.</param>
        /// <param name="stream">Text Stream.</param>
        /// <param name="values">Name of colums that need to print.</param>
        /// <exception cref="ArgumentNullException">Thrown when records, stream, values or culture is null.</exception>
        /// <exception cref="ArgumentException">Thrown when records is empty.</exception>
        public static void WriteToTextSream<T>(IEnumerable<T> records, TextWriter stream, string[] values)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records), "Records can't be null.");
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream), "Stream can't be null.");
            }

            if (values is null)
            {
                throw new ArgumentNullException(nameof(values), "Values can't be null.");
            }

            List<T> list = new List<T>(records);

            if (list.Count == 0)
            {
                throw new ArgumentException("Records are empty.", nameof(records));
            }

            Type recordType = list[0].GetType();
            var fields = recordType.GetProperties();
            List<PropertyInfo> neededFields = new List<PropertyInfo>();
            if (values.Length == 1 && values[0] == "*")
            {
                neededFields = fields.ToList();
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    bool hasGot = false;
                    foreach (var f in fields)
                    {
                        if (string.Equals(values[i], f.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            neededFields.Add(f);
                            hasGot = true;
                            break;
                        }
                    }

                    if (!hasGot)
                    {
                        throw new ArgumentException($"Can't found parameter {values[i]}.", nameof(values));
                    }
                }
            }

            string[][] fieldsToPrint = new string[list.Count + 1][];
            for (int i = 0; i < fieldsToPrint.Length; i++)
            {
                fieldsToPrint[i] = new string[neededFields.Count];
            }

            bool[] isStringOrChar = new bool[neededFields.Count];
            int[] fieldMaxLength = new int[neededFields.Count];

            for (int i = 0; i < neededFields.Count; i++)
            {
                var firstField = neededFields[i].GetValue(list[0]);
                if (firstField is string || firstField is char)
                {
                    isStringOrChar[i] = true;
                }
                else
                {
                    isStringOrChar[i] = false;
                }

                bool isDate = firstField is DateTime;
                fieldsToPrint[0][i] = neededFields[i].Name;
                if (isDate)
                {
                    fieldsToPrint[1][i] = ((DateTime)firstField).ToString("yyyy - MMM - dd", CultureInfo.InvariantCulture);
                }
                else
                {
                    fieldsToPrint[1][i] = firstField.ToString();
                }

                fieldMaxLength[i] = Math.Max(fieldsToPrint[1][i].Length, fieldsToPrint[0][i].Length);
                for (int j = 1; j < list.Count; j++)
                {
                    if (isDate)
                    {
                        fieldsToPrint[j + 1][i] = ((DateTime)neededFields[i].GetValue(list[j])).ToString("yyyy - MMM - dd", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        fieldsToPrint[j + 1][i] = neededFields[i].GetValue(list[j]).ToString();
                    }

                    if (fieldsToPrint[j + 1][i].Length > fieldMaxLength[i])
                    {
                        fieldMaxLength[i] = fieldsToPrint[j + 1][i].Length;
                    }
                }
            }

            string delimiter = GetDelimiter(fieldMaxLength);

            for (int i = 0; i < list.Count + 1; i++)
            {
                stream.WriteLine(delimiter);
                for (int j = 0; j < neededFields.Count; j++)
                {
                    stream.Write('|');
                    if (isStringOrChar[j])
                    {
                        stream.Write(fieldsToPrint[i][j]);
                        stream.Write(PrintSimbol(fieldMaxLength[j] - fieldsToPrint[i][j].Length, ' '));
                    }
                    else
                    {
                        stream.Write(PrintSimbol(fieldMaxLength[j] - fieldsToPrint[i][j].Length, ' '));
                        stream.Write(fieldsToPrint[i][j]);
                    }
                }

                stream.WriteLine('|');
            }

            stream.Write(delimiter + '\n');
        }

        private static string GetDelimiter(int[] fieldMaxLength)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < fieldMaxLength.Length; i++)
            {
                result.Append('+');
                result.Append(PrintSimbol(fieldMaxLength[i], '-'));
            }

            result.Append('+');
            return result.ToString();
        }

        private static string PrintSimbol(int count, char symbol)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                result.Append(symbol);
            }

            return result.ToString();
        }
    }
}
