namespace UniT.Utilities
{
    using System.Collections.Generic;
    using System.Linq;

    public class PriorityQueue<TElement, TPriority>
    {
        private readonly SortedList<TPriority, TElement> items;

        public PriorityQueue() : this(Comparer<TPriority>.Default)
        {
        }

        public PriorityQueue(IComparer<TPriority> comparer)
        {
            this.items = new(Comparer<TPriority>.Create((i1, i2) =>
            {
                var result = comparer.Compare(i2, i1);
                return result != 0 ? result : 1;
            }));
        }

        public int Count => this.items.Count;

        public void Enqueue(TElement element, TPriority priority)
        {
            this.items.Add(priority, element);
        }

        public TElement Dequeue()
        {
            var result = this.Peek();
            this.items.RemoveAt(0);
            return result;
        }

        public TElement Peek()
        {
            return this.items.First().Value;
        }
    }
}