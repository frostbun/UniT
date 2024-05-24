#nullable enable
namespace UniT.Utilities
{
    using System.Collections.Generic;

    public class Deque<T>
    {
        private readonly LinkedList<T> list = new LinkedList<T>();

        public int Count => this.list.Count;

        public void PushFront(T item)
        {
            this.list.AddFirst(item);
        }

        public void PushBack(T item)
        {
            this.list.AddLast(item);
        }

        public T PopFront()
        {
            var item = this.list.First.Value;
            this.list.RemoveFirst();
            return item;
        }

        public T PopBack()
        {
            var item = this.list.Last.Value;
            this.list.RemoveLast();
            return item;
        }

        public T PeekFront()
        {
            return this.list.First.Value;
        }

        public T PeekBack()
        {
            return this.list.Last.Value;
        }
    }
}