namespace UniT.Addressables
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    public interface IAddressableManager
    {
        public UniTask<T> Load<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask<T> LoadComponent<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Component;

        public void Unload(string key);

        public void Unload<T>();

        public UniTask<SceneInstance> LoadScene(string sceneName, string key = null, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask UnloadScene(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }
}