using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Iterator
{
    public class FilesystemIterator : IRecordIterator
    {
        private Func<int, FileCabinetRecord> getRecord;
        private long length;
        private int currentElement = -1;

        public FilesystemIterator(long length, Func<int, FileCabinetRecord> getRecord)
        {
            this.getRecord = getRecord;
            this.length = length;
        }

        public FileCabinetRecord GetNext()
        {
            if ((this.currentElement + 1) * 278 < this.length)
            {
                this.currentElement++;
            }

            return this.getRecord(this.currentElement);
        }

        public bool HasMore()
        {
            if ((this.currentElement + 1) * 278 < this.length)
            {
                return false;
            }

            return true;
        }
    }
}
