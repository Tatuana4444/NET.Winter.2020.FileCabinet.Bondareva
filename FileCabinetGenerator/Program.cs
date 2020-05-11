using FileCabinetApp;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Class that gets generation parameters from user and generates records.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Gets  generation parameters and generates records.
        /// </summary>
        /// <param name="args">Arguments from console runs.</param>
        public static void Main(string[] args)
        {
            bool isCsv = false;
            string outputFile = "1.xml";
            int amount = 100;
            int startId = 1;

            int i = 0;
            while(i < args.Length)
            {
                string[] param = args[i].Split('=');
                switch (param[0])
                {
                    case "-t":
                        if (args[++i] == "csv")
                        {
                            isCsv = true;
                        }
                        break;
                    case "-o":
                        outputFile = args[++i];
                        break;
                    case "-a":
                        amount = int.Parse(args[++i]);
                        break;
                    case "-i":
                        startId = int.Parse(args[++i]);
                        break;
                    case "--output-type":
                        if (param[1] == "csv")
                        {
                            isCsv = true;
                        }
                        break;
                    case "--output":
                        outputFile = param[1];
                        break;
                    case "--records-amount":
                        amount = int.Parse(param[1]);
                        break;
                    case "--start-id":
                        startId = int.Parse(param[1]);
                        break;
                }

                i++;
            }

            if ((string.Equals(Path.GetExtension(outputFile)[1..], "csv", StringComparison.InvariantCultureIgnoreCase) && isCsv) ||
                (string.Equals(Path.GetExtension(outputFile)[1..], "xml", StringComparison.InvariantCultureIgnoreCase) && !isCsv))
            {
                RecordForSerializer list = GenerateData(amount, startId);
                try
                {
                    TextWriter writer = new StreamWriter(outputFile);
                    if (isCsv)
                    {
                        foreach (var record in list.Record)
                        {
                            writer.WriteLine($"{record.Id}, {record.Name.FirstName}, {record.Name.LastName}, " +
                            $"{record.DateOfBirth}, {record.Gender}, {record.PassportId}, {record.Salary}");
                        }
                        writer.Flush();
                    }
                    else
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(RecordForSerializer));
                        ser.Serialize(writer, list);
                    }
                }
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine($"{amount} records wrote to {outputFile} start with {startId} index.");
            }
            else
            {
                Console.WriteLine("Incorrect file extension.");
            }
        }

        private static RecordForSerializer GenerateData(int amount, int startId)
        {
            Random rnd = new Random();
            DateTime date = new DateTime(1950, 1, 1);
            DateTime currentDate = DateTime.Now;
            RecordForSerializer list = new RecordForSerializer();
            for (int i = startId; i < startId + amount; i++)
            {

                Record record = new Record(i, 
                    new Name { FirstName = GenerateString(rnd.Next(2, 60)), LastName = GenerateString(rnd.Next(2, 60))},
                    date.AddDays(rnd.Next((currentDate - date).Days)), 
                    rnd.Next(1, 3) == 1 ? 'M' : 'W', 
                    (short)rnd.Next(1000, 9999), 
                    rnd.Next(375, int.MaxValue));
                list.Record.Add(record);
            }
            
            return list;
        }

        private static string GenerateString(int count)
        {
            Random rnd = new Random();
            StringBuilder result = new StringBuilder();
            for(int i = 0; i < count; i++)
            {
                result.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rnd.NextDouble() + 65))));
            }

            return result.ToString();
        }
    }
}
