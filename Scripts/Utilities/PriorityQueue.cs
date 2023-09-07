namespace UniT.Utilities
{
    using System;
    using System.Collections.Generic;

    public class PriorityQueue<TElement, TPriority>
    {
        private readonly SortedList<TPriority, TElement> _items;

        public PriorityQueue() : this(Comparer<TPriority>.Default)
        {
        }

        public PriorityQueue(Comparison<TPriority> comparison) : this(Comparer<TPriority>.Create(comparison))
        {
        }

        public PriorityQueue(IComparer<TPriority> comparer)
        {
            this._items = new(Comparer<TPriority>.Create((i1, i2) =>
            {
                var result = comparer.Compare(i1, i2);
                return result != 0 ? result : 1;
            }));
        }

        public int Count => this._items.Count;

        public void Enqueue(TElement element, TPriority priority)
        {
            this._items.Add(priority, element);
        }

        public TElement Dequeue()
        {
            var result = this._items.Values[this._items.Count - 1];
            this._items.RemoveAt(this._items.Count - 1);
            return result;
        }

        public TElement Peek()
        {
            return this._items.Values[this._items.Count - 1];
        }
    }
}