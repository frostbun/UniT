namespace UniT.Assets
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;

    public sealed class AddressableSceneManager : ISceneManager
    {
        #region Constructor

        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> loadedScenes;
        private readonly ILogger                                                 logger;

        [Preserve]
        public AddressableSceneManager(ILogger logger = null)
        {
            this.loadedScenes = new();
            this.logger       = logger ?? ILogger.Default(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this.logger.Config;

        public UniTask<SceneInstance> LoadScene(string sceneName, string key = null, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (this.loadedScenes.ContainsKey(key ??= sceneName))
                throw new ArgumentException($"Duplicate key {key} found");
            if (!activateOnLoad)
                this.logger.Warning("Set `activateOnLoad` to false will block all other `AsyncOperationHandle` until the scene is activated");
            return this.loadedScenes
                .GetOrAdd(key, () => Addressables.LoadSceneAsync(sceneName, loadMode, activateOnLoad, priority))
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(scene =>
                {
                    if (loadMode is LoadSceneMode.Single)
                        this.loadedScenes.RemoveAll((oldKey, _) => oldKey != key);
                    this.logger.Debug($"Loaded scene {key}");
                    return scene;
                });
        }

        public UniTask UnloadScene(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (!this.loadedScenes.Remove(key, out var scene))
            {
                this.logger.Warning($"Trying to unload scene {key} that was not loaded");
                return UniTask.CompletedTask;
            }
            return Addressables.UnloadSceneAsync(scene)
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(_ => this.logger.Debug($"Unloaded scene {key}"));
        }

        #endregion
    }
}