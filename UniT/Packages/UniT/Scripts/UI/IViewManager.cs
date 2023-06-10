namespace UniT.UI
{
    public interface IViewManager
    {
        public interface IViewInstance
        {
            public enum Status
            {
                Closed,
                Hidden,
                Stacking,
                Floating,
                Detached,
            }

            public Status CurrentStatus { get; }

            public void Stack();

            public void Float();

            public void Detach();

            public void Hide();

            public void Close();
        }
    }
}