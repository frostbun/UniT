namespace UniT.ResourceManager
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    public interface ISceneManager
    {
        public LogConfig LogConfig { get; }

        public UniTask<SceneInstance> LoadScene(string sceneName, string key = null, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask UnloadScene(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }
}