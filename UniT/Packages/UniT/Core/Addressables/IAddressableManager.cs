namespace UniT.Core.Addressables
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine.ResourceManagement.ResourceProviders;

    public interface IAddressableManager
    {
        public UniTask<T> Load<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask<T> LoadOnce<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask<SceneInstance> LoadScene(string key, bool activateOnLoad = true, int priority = 100, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public void Release(string key);
    }
}