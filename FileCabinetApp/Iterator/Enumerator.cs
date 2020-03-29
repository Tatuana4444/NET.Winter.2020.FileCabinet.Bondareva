using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Iterator
{
    public class Enumerator<T> : IEnumerator<T>, IDisposable
    {
        private Func<int, T> getRecord;
        private long length;
        private int currentIndex;

        public Enumerator(long length, Func<int, T> getRecord)
        {
            this.getRecord = getRecord;
            this.length = length;
            this.currentIndex = -1;
        }

        public T Current
        {
            get
            {
                if (this.currentIndex < 0)
                {
                    throw new InvalidOperationException();
                }

                return this.getRecord(this.currentIndex);
            }
        }

        object IEnumerator.Current => this.Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if ((this.currentIndex + 1) * 278 < this.length)
            {
                this.currentIndex++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            this.currentIndex = -1;
        }
    }
}
