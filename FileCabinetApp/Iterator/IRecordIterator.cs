using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public interface IRecordIterator
    {
        public FileCabinetRecord GetNext();

        public bool HasMore();
    }
}
