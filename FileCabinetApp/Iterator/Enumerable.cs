using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Iterator
{
    public class Enumerable<T> : IEnumerable<T>
    {
        private Func<int, T> getT;
        private long length;

        public Enumerable(long length, Func<int, T> getT)
        {
            this.getT = getT;
            this.length = length;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator<T>(this.length, this.getT);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
