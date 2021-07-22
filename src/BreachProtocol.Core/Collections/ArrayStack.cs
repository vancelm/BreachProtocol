using System;
using System.Collections;
using System.Collections.Generic;

namespace BreachProtocol.Collections
{
    /// <summary>
    /// Represents a fixed-length stack backed by an array whose elements can be read by index.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in this ArrayStack</typeparam>
    public class ArrayStack<T> : IReadOnlyList<T>
    {
        private readonly T[] _array;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _array[index];
            }
        }

        /// <summary>
        /// Gets the maximum number of elements the <see cref="ArrayStack{T}"/> can hold.
        /// </summary>
        public int Capacity => _array.Length;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ArrayStack{T}"/>.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ArrayStack{T}"/> with the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity of the <see cref="ArrayStack{T}"/>.</param>
        public ArrayStack(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _array = new T[capacity];
        }

        /// <summary>
        /// Adds an element to the top of the <see cref="ArrayStack{T}"/>.
        /// </summary>
        /// <param name="item">The element to push onto the <see cref="ArrayStack{T}"/>.</param>
        public void Push(T item)
        {
            if (Count == Capacity)
                throw new InvalidOperationException("The buffer is full.");

            _array[Count] = item;
            Count++;
        }

        /// <summary>
        /// Returns the element at the top of the <see cref="ArrayStack{T}"/> without removing it.
        /// </summary>
        /// <returns>The element at the top of the <see cref="ArrayStack{T}"/>.</returns>
        public T Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException("The buffer is empty.");

            return _array[Count - 1];
        }

        /// <summary>
        /// Removes and returns the element at the top of the <see cref="ArrayStack{T}"/>.
        /// </summary>
        /// <returns>The element removed from the top of the <see cref="ArrayStack{T}"/>.</returns>
        public T Pop()
        {
            if (Count == 0)
                throw new InvalidOperationException("The buffer is empty.");

            T item = _array[Count - 1];
            Count--;
            _array[Count] = default;
            return item;
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="ArrayStack{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="ArrayStack{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (Count == 0)
                yield break;

            for (int i = 0; i < Count; i++)
            {
                yield return _array[i];
            }
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="ArrayStack{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the <see cref="ArrayStack{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
