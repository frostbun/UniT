#if UNIT_ADDRESSABLES
namespace UniT.ResourcesManager
{
    using UniT.Logging;
    using UnityEngine.AddressableAssets;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public sealed class AddressableScenesManager : IScenesManager
    {
        #region Constructor

        private readonly ILogger logger;

        [Preserve]
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
        #endif
    }
}
#endif