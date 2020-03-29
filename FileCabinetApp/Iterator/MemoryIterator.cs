using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp.Iterator
{
    public class MemoryIterator : IRecordIterator
    {
        private int currentElement = -1;

        private ReadOnlyCollection<FileCabinetRecord> list;

        public MemoryIterator(ReadOnlyCollection<FileCabinetRecord> list)
        {
            this.list = list;
        }

        public FileCabinetRecord GetNext()
        {
            if (this.currentElement + 1 < this.list.Count - 1)
            {
                this.currentElement++;
            }

            return this.list[this.currentElement];
        }

        public bool HasMore()
        {
            if (this.currentElement + 1 < this.list.Count - 1)
            {
                return false;
            }

            return true;
        }
    }
}
