using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetService with timer.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">Service that needs to use.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Create a new record and print tick that it took.
        /// </summary>
        /// <param name="recordData">User's data.</param>
        /// <returns>Id of a new record.</returns>
        public int CreateRecord(RecordData recordData)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.CreateRecord(recordData);
            stopWatch.Stop();
            Console.WriteLine($"Create method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Delete record by parameters.
        /// </summary>
        /// <param name="param">Record parameters.</param>
        /// <returns>List of id recored, that was deleted.</returns>
        public IEnumerable<int> Delete(string param)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.Delete(param);
            stopWatch.Stop();
            Console.WriteLine($"Delete method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Finds records by DateOfBirth and print tick that it took.
        /// </summary>
        /// <param name="dateOfBirth">User's date of Birth.</param>
        /// <returns>Records whith sought-for date of Birth.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.FindByDateOfBirth(dateOfBirth);
            stopWatch.Stop();
            Console.WriteLine($"Find by date of birth method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Finds records by first name and print tick that it took.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <returns>Records whith sought-for firstName.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.FindByFirstName(firstName);
            stopWatch.Stop();
            Console.WriteLine($"Find by first name method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Finds records by last name and print tick that it took.
        /// </summary>
        /// <param name="lastName">User's last name.</param>
        /// <returns>Records whith sought-for lastName.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.FindByFirstName(lastName);
            stopWatch.Stop();
            Console.WriteLine($"Find by last name method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Gets all records and print tick that it took.
        /// </summary>
        /// <returns>All records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.GetRecords();
            stopWatch.Stop();
            Console.WriteLine($"Get records method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Gets count of records  and count of deleted records and print tick that it took.
        /// </summary>
        /// <returns>Count of records  and count of deleted records.</returns>
        public Tuple<int, int> GetStat()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.GetStat();
            stopWatch.Stop();
            Console.WriteLine($"Get stat method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Defragment the data file and print tick that it took.
        /// </summary>
        /// <returns>Count of defragmented records.</returns>
        public int Purge()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = this.service.Purge();
            stopWatch.Stop();
            Console.WriteLine($"Purge method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Restore date from snapshot and print tick that it took.
        /// </summary>
        /// <param name="snapshot">Snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.service.Restore(snapshot);
            stopWatch.Stop();
            Console.WriteLine($"Restore method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Update records by parameters.
        /// </summary>
        /// <param name="param">Record parameters.</param>
        public void Update(string param)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            this.service.Update(param);
            stopWatch.Stop();
            Console.WriteLine($"Edit method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }
    }
}
