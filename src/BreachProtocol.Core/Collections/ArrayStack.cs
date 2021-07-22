using System;
using System.Collections;
using System.Collections.Generic;

namespace BreachProtocol.Collections
{
    public class ArrayStack<T> : IReadOnlyList<T>
    {
        private readonly T[] _array;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _array[index];
            }
        }

        public int Capacity => _array.Length;

        public int Count { get; private set; }

        public ArrayStack(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _array = new T[capacity];
        }

        public void Push(T item)
        {
            if (Count == Capacity)
                throw new InvalidOperationException("The buffer is full.");

            _array[Count] = item;
            Count++;
        }

        public T Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException("The buffer is empty.");

            return _array[Count - 1];
        }

        public T Pop()
        {
            if (Count == 0)
                throw new InvalidOperationException("The buffer is empty.");

            T item = _array[Count - 1];
            Count--;
            _array[Count] = default;
            return item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (Count == 0)
                yield break;

            for (int i = 0; i < Count; i++)
            {
                yield return _array[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
