﻿using FileCabinetApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    class Program
    {
        
        static void Main(string[] args)
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

            List<FileCabinetRecord> list = GenerateData(amount, startId);

            TextWriter writer = new StreamWriter(outputFile);
            if (isCsv)
            {
                
                foreach (var record in list)
                {
                    writer.WriteLine($"{record.Id}, {record.FirstName}, {record.LastName}, " +
                    $"{record.DateOfBirth}, {record.Gender}, {record.PassportId}, {record.Salary}");
                }
                writer.Flush();
            }
            else
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<FileCabinetRecord>));
                    ser.Serialize(writer, list);

            }
        }

        private static List<FileCabinetRecord> GenerateData(int amount, int startId)
        {
            Random rnd = new Random();
            DateTime date = new DateTime(1950, 1, 1);
            DateTime currentDate = DateTime.Now;
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            for (int i = startId; i < startId + amount; i++)
            {

                FileCabinetRecord record = new FileCabinetRecord
                {
                    Id = i,
                    FirstName = GenerateString(rnd.Next(2, 60)),
                    LastName = GenerateString(rnd.Next(2, 60)),
                    DateOfBirth = date.AddDays(rnd.Next((currentDate - date).Days)),
                    Gender = rnd.Next(1, 2) == 1 ? 'M' : 'W',
                    PassportId = (short)rnd.Next(1000, 9999),
                    Salary = rnd.Next(375, int.MaxValue),
                };

                list.Add(record);
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
