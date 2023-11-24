namespace UniT.ResourcesManager
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public interface IAssetsManager : IDisposable
    {
        public static Func<IAssetsManager> Default { get; set; } = () => new AddressableAssetsManager();

        public LogConfig LogConfig { get; }

        public T Load<T>(string key) where T : Object;

        public UniTask<T> LoadAsync<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Object;

        public void Unload(string key);

        public UniTask<T> LoadComponent<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default) =>
            this.LoadAsync<GameObject>(key, progress, cancellationToken)
                .ContinueWith(gameObject =>
                    gameObject.GetComponent<T>()
                    ?? throw new InvalidOperationException($"Component {typeof(T).Name} not found in GameObject {key}")
                );

        #region Implicit Key

        public T Load<T>() where T : Object => this.Load<T>(typeof(T).GetKey());

        public UniTask<T> LoadAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Object => this.LoadAsync<T>(typeof(T).GetKey(), progress, cancellationToken);

        public UniTask<T> LoadComponent<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) => this.LoadComponent<T>(typeof(T).GetKey(), progress, cancellationToken);

        public void Unload<T>() => this.Unload(typeof(T).GetKey());

        #endregion
    }
}