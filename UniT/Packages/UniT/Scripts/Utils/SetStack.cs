namespace UniT.Utils
{
    using System;
    using System.Collections.Generic;

    public class SetStack<T>
    {
        private readonly List<T> list;

        public SetStack()
        {
            this.list = new();
        }

        public int Count => this.list.Count;

        public bool IsEmpty => this.list.Count < 1;

        public bool Push(T item)
        {
            var isFirstTime = !this.list.Remove(item);
            this.list.Add(item);
            return isFirstTime;
        }

        public T Pop()
        {
            var item = this.Peek();
            this.list.RemoveAt(this.Count - 1);
            return item;
        }

        public T Peek()
        {
            if (this.IsEmpty) throw new InvalidOperationException("Stack is empty");
            return this.list[^1];
        }

        public bool Remove(T item)
        {
            return this.list.Remove(item);
        }
    }
}