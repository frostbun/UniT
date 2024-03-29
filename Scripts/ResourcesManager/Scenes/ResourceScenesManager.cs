﻿namespace UniT.ResourcesManager
{
    using System;
    using UniT.Logging;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    [Preserve]
    public sealed class ResourceScenesManager : IScenesManager, IHasLogger
    {
        #region Constructor

        private readonly ILogger logger;

        public ResourceScenesManager(ILogger.IFactory loggerFactory)
        {
            this.logger = loggerFactory.Create(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        LogConfig IHasLogger.LogConfig => this.logger.Config;

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
        #else
        IEnumerator IScenesManager.LoadSceneAsync(string sceneName, LoadSceneMode loadMode, Action callback, IProgress<float> progress)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName, loadMode);
            while (!operation.isDone)
            {
                progress?.Report(operation.progress);
                yield return null;
            }
            this.logger.Debug($"Loaded {sceneName}");
            callback?.Invoke();
        }
        #endif
    }
}