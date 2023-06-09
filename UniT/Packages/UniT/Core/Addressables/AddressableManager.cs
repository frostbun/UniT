namespace UniT.Core.Addressables
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Core.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using ILogger = UniT.Core.Logging.ILogger;

    public class AddressableManager : IAddressableManager
    {
        private readonly ILogger                                  logger;
        private readonly Dictionary<string, AsyncOperationHandle> handleCache;
        private readonly Dictionary<string, SceneInstance>        loadedScenes;

        public AddressableManager(ILogger logger)
        {
            this.logger       = logger;
            this.handleCache  = new();
            this.loadedScenes = new();
            this.logger.Info($"{this.GetType().Name} instantiated", Color.green);
        }

        public UniTask<T> Load<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return this.handleCache.GetOrDefault(key, () => Addressables.LoadAssetAsync<T>(key))
                       .Convert<T>()
                       .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                       .ContinueWith(asset =>
                       {
                           this.logger.Info($"Loaded addressable {key}");
                           progress?.Report(1);
                           return asset;
                       });
        }

        public void Release(string key)
        {
            if (!this.handleCache.Remove(key, out var handle))
            {
                this.logger.Warning("Trying to release an addressable that was not loaded");
                return;
            }

            Addressables.Release(handle);
            this.logger.Info($"Released addressable {key}");
        }

        public UniTask LoadScene(string sceneName, string key = null, LoadSceneMode loadMode = LoadSceneMode.Single, int priority = 100, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (this.loadedScenes.ContainsKey(key ??= sceneName))
            {
                throw new InvalidOperationException("Key already exists in loaded scenes");
            }

            return Addressables.LoadSceneAsync(sceneName, loadMode: loadMode, priority: priority)
                               .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                               .ContinueWith(scene =>
                               {
                                   if (loadMode is LoadSceneMode.Single)
                                   {
                                       this.loadedScenes.Values.ForEach(Addressables.Release);
                                       this.loadedScenes.Clear();
                                   }

                                   this.loadedScenes.Add(key, scene);
                                   this.logger.Info($"Loaded scene {key}");
                                   progress?.Report(1);
                               });
        }

        public UniTask UnloadScene(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (!this.loadedScenes.Remove(key, out var scene))
            {
                this.logger.Warning("Trying to unload a scene that was not loaded");
                return UniTask.CompletedTask;
            }

            return Addressables.UnloadSceneAsync(scene)
                               .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                               .ContinueWith(_ =>
                               {
                                   this.logger.Info($"Unloaded scene {key}");
                                   progress?.Report(1);
                               });
        }
    }
}