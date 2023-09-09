namespace UniT
{
    public interface IInitializable
    {
        public void Initialize();
    }

    public interface IEarlyInitializable
    {
        public void Initialize();
    }

    public interface ILateInitializable
    {
        public void Initialize();
    }
}