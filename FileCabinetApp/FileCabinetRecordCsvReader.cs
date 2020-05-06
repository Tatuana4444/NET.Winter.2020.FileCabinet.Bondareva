using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Reader for csv.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="stream">Current Stream.</param>
        public FileCabinetRecordCsvReader(StreamReader stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Read all records from stream.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            var validator = new ValidatorBuilder().CreateDefault();
            string rec = this.stream.ReadLine();
            do
            {
                string[] elements = rec.Split(", ");
                if (int.TryParse(elements[0], out int id)
                    && DateTime.TryParse(elements[3], out DateTime dateOfBirth)
                    && char.TryParse(elements[4], out char gender)
                    && short.TryParse(elements[5], out short pasportId)
                    && decimal.TryParse(elements[6], out decimal salary))
                {
                    RecordData recordData = new RecordData(elements[1], elements[2], dateOfBirth, gender, pasportId, salary);
                    try
                    {
                        validator.ValidateParameters(recordData);
                        var record = new FileCabinetRecord
                        {
                            Id = id,
                            FirstName = recordData.FirstName,
                            LastName = recordData.LastName,
                            DateOfBirth = recordData.DateOfBirth,
                            Gender = recordData.Gender,
                            PassportId = recordData.PassportId,
                            Salary = recordData.Salary,
                        };
                        list.Add(record);
                    }
                    catch (ArgumentNullException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                rec = this.stream.ReadLine();
            }
            while (rec != null);

            return list;
        }
    }
}
