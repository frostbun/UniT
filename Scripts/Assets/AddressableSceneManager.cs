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

    public class AddressableSceneManager
    {
        #region Constructor

        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _loadedScenes;
        private readonly ILogger                                                 _logger;

        [Preserve]
        public AddressableSceneManager(ILogger logger = null)
        {
            this._loadedScenes = new();
            this._logger       = logger ?? ILogger.Default(this.GetType().Name);
        }

        #endregion

        #region Public

        public UniTask<SceneInstance> LoadScene(string sceneName, string key = null, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (this._loadedScenes.ContainsKey(key ??= sceneName))
            {
                var exception = new InvalidOperationException($"Key {key} already exists in loaded scenes");
                this._logger.Exception(exception);
                throw exception;
            }
            if (!activateOnLoad)
            {
                this._logger.Warning("Set `activateOnLoad` to false will block all other `AsyncOperationHandle` until the scene is activated");
            }
            return (this._loadedScenes[key] = Addressables.LoadSceneAsync(sceneName, loadMode, activateOnLoad, priority))
                   .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                   .ContinueWith(scene =>
                   {
                       if (loadMode is LoadSceneMode.Single)
                       {
                           this._loadedScenes.RemoveAll((oldKey, _) => oldKey != key);
                       }
                       this._logger.Debug($"Loaded scene {key}");
                       return scene;
                   });
        }

        public UniTask UnloadScene(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (!this._loadedScenes.Remove(key, out var scene))
            {
                this._logger.Warning($"Trying to unload scene {key} that was not loaded");
                return UniTask.CompletedTask;
            }
            return Addressables.UnloadSceneAsync(scene)
                               .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                               .ContinueWith(_ => this._logger.Debug($"Unloaded scene {key}"));
        }

        #endregion
    }
}