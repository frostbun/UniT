namespace UniT.Assets
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using ILogger = UniT.Logging.ILogger;

    public class AddressablesManager : IAssetsManager
    {
        public ILogger Logger { get; }

        private readonly Dictionary<string, AsyncOperationHandle>                loadedAssets;
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> loadedScenes;

        public AddressablesManager(ILogger logger = null)
        {
            this.loadedAssets = new();
            this.loadedScenes = new();
            this.Logger       = logger ?? ILogger.Default(this.GetType().Name);
        }

        public UniTask<T> Load<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            key ??= typeof(T).GetKey();
            return this.loadedAssets.GetOrAdd(key, () => Addressables.LoadAssetAsync<T>(key))
                       .Convert<T>()
                       .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                       .ContinueWith(asset =>
                       {
                           this.Logger.Debug($"Loaded asset {key}");
                           progress?.Report(1);
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
                               throw this.Logger.Exception(new InvalidOperationException($"Component {typeof(T).Name} not found in GameObject {gameObject.name}"));
                           }
                           return component;
                       });
        }

        public void Unload(string key)
        {
            if (!this.loadedAssets.Remove(key, out var handle))
            {
                this.Logger.Warning($"Trying to unload asset {key} that was not loaded");
                return;
            }
            Addressables.Release(handle);
            this.Logger.Debug($"Unloaded asset {key}");
        }

        public void Unload<T>()
        {
            this.Unload(typeof(T).GetKey());
        }

        public UniTask<SceneInstance> LoadScene(string sceneName, string key = null, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (this.loadedScenes.ContainsKey(key ??= sceneName))
            {
                throw this.Logger.Exception(new InvalidOperationException("Key already exists in loaded scenes"));
            }
            if (!activateOnLoad)
            {
                this.Logger.Warning("Set `activateOnLoad` to false will block all other `AsyncOperationHandle` until the scene is activated");
            }
            return (this.loadedScenes[key] = Addressables.LoadSceneAsync(sceneName, loadMode, activateOnLoad, priority))
                   .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                   .ContinueWith(scene =>
                   {
                       if (loadMode is LoadSceneMode.Single)
                       {
                           this.loadedScenes.RemoveAll((oldKey, _) => oldKey != key);
                       }
                       this.Logger.Debug($"Loaded scene {key}");
                       return scene;
                   });
        }

        public UniTask UnloadScene(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (!this.loadedScenes.Remove(key, out var scene))
            {
                this.Logger.Warning($"Trying to unload scene {key} that was not loaded");
                return UniTask.CompletedTask;
            }
            return Addressables.UnloadSceneAsync(scene)
                               .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                               .ContinueWith(_ => this.Logger.Debug($"Unloaded scene {key}"));
        }
    }
}