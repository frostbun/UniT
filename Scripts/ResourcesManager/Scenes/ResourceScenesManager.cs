namespace UniT.ResourcesManager
{
    using UniT.Logging;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public class ResourceScenesManager : IScenesManager
    {
        #region Constructor

        private readonly ILogger logger;

        [Preserve]
        public ResourceScenesManager(ILogger logger)
        {
            this.logger = logger;
            this.logger.Debug("Constructed");
        }

        #endregion

        LogConfig IScenesManager.LogConfig => this.logger.Config;

        void IScenesManager.LoadScene(string sceneName, LoadSceneMode loadMode)
        {
            SceneManager.LoadScene(sceneName, loadMode);
            this.logger.Debug($"Loaded {sceneName}");
        }

        #if UNIT_UNITASK
        UniTask IScenesManager.LoadSceneAsync(string sceneName, LoadSceneMode loadMode, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return SceneManager.LoadSceneAsync(sceneName, loadMode)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(() => this.logger.Debug($"Loaded {sceneName}"));
        }
        #endif
    }
}