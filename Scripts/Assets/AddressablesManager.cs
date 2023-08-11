namespace UniT.Assets
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public class AddressablesManager : IAssetsManager
    {
        #region Constructor

        private readonly Dictionary<string, AsyncOperationHandle>                _loadedAssets;
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _loadedScenes;
        private readonly ILogger                                                 _logger;

        [Preserve]
        public AddressablesManager(ILogger logger = null)
        {
            this._loadedAssets = new();
            this._loadedScenes = new();
            this._logger       = logger ?? ILogger.Default(this.GetType().Name);
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this._logger.Config;

        public UniTask<T> Load<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            key ??= typeof(T).GetKey();
            return this._loadedAssets.GetOrAdd(key, () => Addressables.LoadAssetAsync<T>(key))
                       .Convert<T>()
                       .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                       .ContinueWith(asset =>
                       {
                           this._logger.Debug($"Loaded asset {key}");
                           return asset;
                       });
        }

        public UniTask<T> LoadComponent<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Component
        {
            return this.Load<GameObject>(key ?? typeof(T).GetKey(), progress, cancellationToken)
                       .ContinueWith(gameObject =>
                       {
                           var component = gameObject.GetComponent<T>();
                           if (component is null)
                           {
                               var exception = new InvalidOperationException($"Component {typeof(T).Name} not found in GameObject {gameObject.name}");
                               this._logger.Exception(exception);
                               throw exception;
                           }
                           return component;
                       });
        }

        public void Unload(string key)
        {
            if (!this._loadedAssets.Remove(key, out var handle))
            {
                this._logger.Warning($"Trying to unload asset {key} that was not loaded");
                return;
            }
            Addressables.Release(handle);
            this._logger.Debug($"Unloaded asset {key}");
        }

        public void Unload<T>()
        {
            this.Unload(typeof(T).GetKey());
        }

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