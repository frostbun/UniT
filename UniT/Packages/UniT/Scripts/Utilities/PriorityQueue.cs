namespace UniT.Utilities
{
    using System.Collections.Generic;

    public class PriorityQueue<TElement, TPriority>
    {
        private readonly SortedList<TPriority, TElement> _items;

        public PriorityQueue() : this(Comparer<TPriority>.Default)
        {
        }

        public PriorityQueue(IComparer<TPriority> comparer)
        {
            this._items = new(Comparer<TPriority>.Create((i1, i2) =>
            {
                var result = comparer.Compare(i2, i1);
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
            var result = this.Peek();
            this._items.RemoveAt(0);
            return result;
        }

        public TElement Peek()
        {
            return this._items.Values[0];
        }
    }
}