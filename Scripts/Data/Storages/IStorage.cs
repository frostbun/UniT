namespace UniT.Data.Storages
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public interface IStorage : IReadOnlyStorage
    {
        public UniTask Save(string[] keys, string[] values);

        public UniTask Flush();
    }
}