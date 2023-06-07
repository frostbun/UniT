namespace UniT.Addressables
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IAddressableManager
    {
        public void Release(string key);

        public UniTask<T> Load<T>(string key, bool cache = false);

        public UniTask<T> Load<T>(Type type, bool cache = false);
    }
}