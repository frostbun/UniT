namespace UniT.ResourcesManager
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UnityEngine.SceneManagement;

    public interface IScenesManager
    {
        public LogConfig LogConfig { get; }

        public void LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single);

        public UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, IProgress<float> progress = null, CancellationToken cancellationToken = default);
    }
}