namespace UniT.Data.Storages
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IReadOnlyStorage
    {
        public bool CanStore(Type type);

        public UniTask<string[]> Load(string[] keys);
    }
}