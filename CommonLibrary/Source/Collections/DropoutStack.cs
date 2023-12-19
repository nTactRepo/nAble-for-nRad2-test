using System;
using System.Collections.Generic;

namespace CommonLibrary.Collections
{
    /// <summary>
    /// This class implements a normal stack that only keeps the last "Count" items.
    /// If already at count, and another item is pushed, it drops the oldest.
    /// Used mainly for limited undo/redo stacks
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropoutStack<T>
    {
        #region Data Members

        private readonly T[] _items;
        private int _top = 0;

        #endregion

        #region Properties

        public int Size { get; }
        public int Count { get; private set; } = 0;
        public bool IsEmpty => Count == 0;

        #endregion

        #region Functions

        #region Constructors

        public DropoutStack(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException($"Stack size was set to {size} -- illegal!");
            }

            Size = size;
            Count = 0;
            _items = new T[Size];
        }

        #endregion

        #region Public Functions

        public void Push(T item)
        {
            _items[_top] = item;
            _top = GetNextTop();

            if (Count < Size)
            {
                Count++;
            }
        }
        public T Pop()
        {
            ThrowIfEmpty();
            _top = GetPreviousTop();
            Count--;
            return _items[_top];
        }

        public T Peek()
        {
            ThrowIfEmpty();
            return _items[GetPreviousTop()];
        }

        public void Clear()
        {
            _top = 0;
            Count = 0;
        }

        public List<T> ToList()
        {
            return new List<T>(_items);
        }

        #endregion

        #region Private Functions

        private int GetNextTop() => (_top + 1) % Size;

        private int GetPreviousTop() => (Size + _top - 1) % Size;

        private void ThrowIfEmpty()
        {
            if (IsEmpty)
            {
                throw new QueueEmptyException("The queue was empty, so peek and pop are illegal.");
            }
        }

        #endregion

        #endregion
    }
}
