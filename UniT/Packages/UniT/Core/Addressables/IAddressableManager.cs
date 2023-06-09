namespace UniT.Core.Addressables
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine.SceneManagement;

    public interface IAddressableManager
    {
        public UniTask<T> Load<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public void Release(string key);

        public UniTask LoadScene(string sceneName, string key = null, LoadSceneMode loadMode = LoadSceneMode.Single, int priority = 100, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask UnloadScene(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }
}