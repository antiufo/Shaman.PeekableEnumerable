using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shaman.Runtime
{
    public class PeekableEnumerator<T> : IDisposable, IEnumerator<T>
    {
        private IEnumerator<T> source;
        private OpenQueue<T> queue;
        private T current;
        private bool hasCurrent;
        private EqualityComparer<T> comparer;


        public PeekableEnumerator(IEnumerable<T> source)
            : this(source.GetEnumerator())
        {
        }

        public PeekableEnumerator(IEnumerator<T> source)
        {
            this.source = source;
            this.queue = new OpenQueue<T>();
            this.comparer = EqualityComparer<T>.Default;
        }

        public bool MoveNext()
        {
            if (queue.Count != 0)
            {
                hasCurrent = true;
                current = queue.Dequeue();
            }
            else
            {
                hasCurrent = source.MoveNext();
                if (hasCurrent)
                    current = source.Current;
            }

            return hasCurrent;
        }

        public T[] Peek(int count)
        {
            var arr = new T[count];
            var i = 0;
            for (i = 0; i < queue.Count && i < count; i++)
            {
                arr[i] = queue[i];
            }

            for (; i < count; i++)
            {
                if (source.MoveNext())
                {
                    queue.Enqueue(source.Current);
                    arr[i] = source.Current;
                }
                else
                {
                    return null;
                }
            }

            return arr;
        }

        public bool ContinuesWith(params T[] items)
        {
            var requestedLength = items.Length;
            var queueSize = queue.Count;
            var i = 0;
            for (i = 0; i < queueSize && i < requestedLength; i++)
            {
                if (!comparer.Equals(items[i], queue[i]))
                    return false;
            }

            for (; i < requestedLength; i++)
            {
                if (source.MoveNext())
                {
                    queue.Enqueue(source.Current);
                    if (!comparer.Equals(items[i], source.Current))
                        return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }


        public T Current
        {
            get
            {
                if (!hasCurrent)
                    throw new InvalidOperationException();
                return current;
            }
        }

        public void Dispose()
        {
            source.Dispose();
        }


        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }


    }
}
