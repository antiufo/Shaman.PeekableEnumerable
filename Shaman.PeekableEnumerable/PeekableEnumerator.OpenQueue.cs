using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
namespace Shaman.Runtime
{

    internal class OpenQueue<T>
    {

        private T[] _array;
        private int _head;
        private int _tail;
        private int _size;
        private static T[] _emptyArray = new T[0];
        private const int _MinimumGrow = 4;
        private const int _GrowFactor = 2;


        public OpenQueue()
        {
            this._array = OpenQueue<T>._emptyArray;
        }

        public OpenQueue(int capacity)
        {
            if (capacity < 0)
            {
                throw new InvalidOperationException();
            }
            this._array = new T[capacity];
            this._head = 0;
            this._tail = 0;
            this._size = 0;
        }


        public void Enqueue(T item)
        {
            if (this._size == this._array.Length)
            {
                int num = this._array.Length * _GrowFactor;
                if (num < this._array.Length + _MinimumGrow)
                {
                    num = this._array.Length + _MinimumGrow;
                }
                this.SetCapacity(num);
            }
            this._array[this._tail] = item;
            this._tail = (this._tail + 1) % this._array.Length;
            this._size++;
        }



        public T Dequeue()
        {
            if (this._size == 0)
            {
                throw new InvalidOperationException();
            }
            T result = this._array[this._head];
            this._array[this._head] = default(T);
            this._head = (this._head + 1) % this._array.Length;
            this._size--;
            return result;
        }


        public T this[int i]
        {
            get
            {
                if (i < 0 || i >= _size)
                    throw new ArgumentOutOfRangeException();
                return this._array[(this._head + i) % this._array.Length];
            }
        }

        public int Count
        {
            get
            {
                return _size;
            }
        }

        private void SetCapacity(int capacity)
        {
            T[] array = new T[capacity];
            if (this._size > 0)
            {
                if (this._head < this._tail)
                {
                    Array.Copy(this._array, this._head, array, 0, this._size);
                }
                else
                {
                    Array.Copy(this._array, this._head, array, 0, this._array.Length - this._head);
                    Array.Copy(this._array, 0, array, this._array.Length - this._head, this._tail);
                }
            }
            this._array = array;
            this._head = 0;
            this._tail = ((this._size == capacity) ? 0 : this._size);
        }


    }
}
