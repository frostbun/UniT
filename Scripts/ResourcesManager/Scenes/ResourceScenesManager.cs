namespace UniT.ResourcesManager
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;

    public class ResourceScenesManager : IScenesManager
    {
        #region Constructor

        private readonly ILogger logger;

        [Preserve]
        public ResourceScenesManager(ILogger logger = null)
        {
            this.logger = logger ?? ILogger.Default(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        LogConfig IScenesManager.LogConfig => this.logger.Config;

        void IScenesManager.LoadScene(string sceneName, LoadSceneMode loadMode)
        {
            SceneManager.LoadScene(sceneName, loadMode);
            this.logger.Debug($"Loaded {sceneName}");
        }

        UniTask IScenesManager.LoadSceneAsync(string sceneName, LoadSceneMode loadMode, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return SceneManager.LoadSceneAsync(sceneName, loadMode)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(() => this.logger.Debug($"Loaded {sceneName}"));
        }
    }
}