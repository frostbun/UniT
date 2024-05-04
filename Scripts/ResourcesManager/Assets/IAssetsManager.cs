namespace UniT.ResourcesManager
{
    using System;
    using UniT.Extensions;
    using UniT.Utilities;
    using UnityEngine;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IAssetsManager
    {
        #region Sync

        public T Load<T>(string key) where T : Object;

        public void Unload(string key);

        public T Load<T>() where T : Object => this.Load<T>(typeof(T).GetKey());

        public void Unload<T>() => this.Unload(typeof(T).GetKey());

        public T LoadComponent<T>(string key) => this.Load<GameObject>(key).GetComponentOrThrow<T>();

        public T LoadComponent<T>() => this.LoadComponent<T>(typeof(T).GetKey());

        #endregion

        #region Async

        #if UNIT_UNITASK
        public UniTask<T> LoadAsync<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Object;

        public UniTask<T> LoadAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Object => this.LoadAsync<T>(typeof(T).GetKey(), progress, cancellationToken);

        public UniTask<T> LoadComponentAsync<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default) =>
            this.LoadAsync<GameObject>(key, progress, cancellationToken)
                .ContinueWith(gameObject => gameObject.GetComponentOrThrow<T>());

        public UniTask<T> LoadComponentAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) => this.LoadComponentAsync<T>(typeof(T).GetKey(), progress, cancellationToken);
        #else
        public IEnumerator LoadAsync<T>(string key, Action<T> callback, IProgress<float> progress = null) where T : Object;

        public IEnumerator LoadAsync<T>(Action<T> callback, IProgress<float> progress = null) where T : Object => this.LoadAsync(typeof(T).GetKey(), callback, progress);

        public IEnumerator LoadComponentAsync<T>(string key, Action<T> callback, IProgress<float> progress = null) =>
            this.LoadAsync<GameObject>(
                key,
                gameObject => callback(gameObject.GetComponentOrThrow<T>()),
                progress
            );

        public IEnumerator LoadComponentAsync<T>(Action<T> callback, IProgress<float> progress = null) => this.LoadComponentAsync(typeof(T).GetKey(), callback, progress);
        #endif

        #endregion
    }
}