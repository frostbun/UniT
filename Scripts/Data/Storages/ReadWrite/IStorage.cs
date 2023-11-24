namespace UniT.Data.Storages
{
    using Cysharp.Threading.Tasks;

    public interface IStorage : IReadOnlyStorage
    {
        public UniTask Save(string[] keys, string[] values);

        public UniTask Flush();
    }
}