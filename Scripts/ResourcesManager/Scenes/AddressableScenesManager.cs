﻿#if UNIT_ADDRESSABLES
namespace UniT.ResourcesManager
{
    using System;
    using UniT.Logging;
    using UnityEngine.AddressableAssets;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    [Preserve]
    public sealed class AddressableScenesManager : IScenesManager, IHasLogger
    {
        #region Constructor

        private readonly ILogger logger;

        public AddressableScenesManager(ILogger.IFactory loggerFactory)
        {
            this.logger = loggerFactory.Create(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        LogConfig IHasLogger.LogConfig => this.logger.Config;

        void IScenesManager.LoadScene(string sceneName, LoadSceneMode loadMode)
        {
            Addressables.LoadSceneAsync(sceneName, loadMode).WaitForCompletion();
            this.logger.Debug($"Loaded {sceneName}");
        }

        #if UNIT_UNITASK
        UniTask IScenesManager.LoadSceneAsync(string sceneName, LoadSceneMode loadMode, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return Addressables.LoadSceneAsync(sceneName, loadMode)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(_ => this.logger.Debug($"Loaded {sceneName}"));
        }
        #else
        IEnumerator IScenesManager.LoadSceneAsync(string sceneName, LoadSceneMode loadMode, Action callback, IProgress<float> progress)
        {
            var operation = Addressables.LoadSceneAsync(sceneName, loadMode);
            while (!operation.IsDone)
            {
                progress?.Report(operation.PercentComplete);
                yield return null;
            }
            this.logger.Debug($"Loaded {sceneName}");
            callback?.Invoke();
        }
        #endif
    }
}
#endif