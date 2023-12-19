using System;
using System.Collections.Generic;

namespace CommonLibrary.Collections
{
    public class EventList<T> : List<T>
    {
        #region Events

        public static event Action Constructed;
        public static event Action<IEnumerable<T>> RangeConstructed;

        public event Action Cleared;

        public event Action<T> Added;
        public event Action<IEnumerable<T>> RangeAdded;

        public event Action<int, T> Inserted;
        public event Action<int, IEnumerable<T>> RangeInserted;

        #endregion

        #region Functions

        #region Constructors

        public EventList()
        {
            Constructed?.Invoke();
        }

        public EventList(int capacity) : base(capacity)
        {
            Constructed?.Invoke();
        }

        public EventList(IEnumerable<T> items) : base(items)
        {
            RangeConstructed?.Invoke(items);
        }

        #endregion

        #region Public Functions

        public new void Clear()
        {
            if (Count > 0)
            {
                base.Clear();
                Cleared?.Invoke();
            }
        }
        
        public new void Add(T item)
        {
            base.Add(item);
            Added?.Invoke(item);
        }

        public new void AddRange(IEnumerable<T> range)
        {
            base.AddRange(range);
            RangeAdded?.Invoke(range);
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            Inserted?.Invoke(index, item);
        }

        public new void InsertRange(int index, IEnumerable<T> range)
        {
            base.InsertRange(index, range);
            RangeInserted?.Invoke(index, range);
        }

        #endregion

        #endregion
    }
}
