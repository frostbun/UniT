namespace UniT.Initializables
{
    public interface IEarlyInitializable
    {
        public void Initialize();
    }

    public interface IInitializable
    {
        public void Initialize();
    }

    public interface ILateInitializable
    {
        public void Initialize();
    }
}