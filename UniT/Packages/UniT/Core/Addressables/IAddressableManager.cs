namespace UniT.Core.Addressables
{
    using Cysharp.Threading.Tasks;
    using UnityEngine.ResourceManagement.ResourceProviders;

    public interface IAddressableManager
    {
        public void Release(string key);

        public UniTask<T> Load<T>(string key, bool cache = false);

        public UniTask<SceneInstance> LoadScene(string key, bool activateOnLoad = true, int priority = 100);
    }
}