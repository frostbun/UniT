namespace UniT.Core.Addressables
{
    using Cysharp.Threading.Tasks;

    public interface IAddressableManager
    {
        public void Release(string key);

        public UniTask<T> Load<T>(string key, bool cache = false);
    }
}