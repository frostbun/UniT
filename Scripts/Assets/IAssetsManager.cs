namespace UniT.Assets
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UnityEngine;

    public interface IAssetsManager : IDisposable
    {
        public static Func<IAssetsManager> Default { get; set; } = () => new AddressableAssetsManager();

        public LogConfig LogConfig { get; }

        public UniTask<T> Load<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask<T> LoadComponent<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Component;

        public void Unload(string key);

        public void Unload<T>();
    }
}