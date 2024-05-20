namespace UniT.ResourcesManager
{
    using System;
    using UniT.Utilities;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IAssetsManager : IDisposable
    {
        #region Sync

        public T Load<T>(string key) where T : Object;

        public bool TryLoad<T>(string key, out T asset) where T : Object;

        public T LoadComponent<T>(string key);

        public bool TryLoadComponent<T>(string key, out T component);

        #region Implicit Key

        public T Load<T>() where T : Object => this.Load<T>(typeof(T).GetKey());

        public bool TryLoad<T>(out T value) where T : Object => this.TryLoad(typeof(T).GetKey(), out value);

        public T LoadComponent<T>() => this.LoadComponent<T>(typeof(T).GetKey());

        public bool TryLoadComponent<T>(out T value) => this.TryLoadComponent(typeof(T).GetKey(), out value);

        #endregion

        #endregion

        #region Async

        #if UNIT_UNITASK
        public UniTask<T> LoadAsync<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Object;

        public UniTask<(bool IsSucceeded, T Asset)> TryLoadAsync<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Object;

        public UniTask<T> LoadComponentAsync<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask<(bool IsSucceeded, T Component)> TryLoadComponentAsync<T>(string key, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        #region Implicit Key

        public UniTask<T> LoadAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Object => this.LoadAsync<T>(typeof(T).GetKey(), progress, cancellationToken);

        public UniTask<(bool IsSucceeded, T Asset)> TryLoadAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Object => this.TryLoadAsync<T>(typeof(T).GetKey(), progress, cancellationToken);

        public UniTask<T> LoadComponentAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) => this.LoadComponentAsync<T>(typeof(T).GetKey(), progress, cancellationToken);

        public UniTask<(bool IsSucceeded, T Component)> TryLoadComponentAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) => this.TryLoadComponentAsync<T>(typeof(T).GetKey(), progress, cancellationToken);

        #endregion

        #else
        public IEnumerator LoadAsync<T>(string key, Action<T> callback, IProgress<float> progress = null) where T : Object;

        public IEnumerator TryLoadAsync<T>(string key, Action<(bool IsSucceeded, T Asset)> callback, IProgress<float> progress = null) where T : Object;

        public IEnumerator LoadComponentAsync<T>(string key, Action<T> callback, IProgress<float> progress = null);

        public IEnumerator TryLoadComponentAsync<T>(string key, Action<(bool IsSucceeded, T Component)> callback, IProgress<float> progress = null);

        #region Implicit Key

        public IEnumerator LoadAsync<T>(Action<T> callback, IProgress<float> progress = null) where T : Object => this.LoadAsync(typeof(T).GetKey(), callback, progress);

        public IEnumerator TryLoadAsync<T>(Action<(bool IsSucceeded, T Asset)> callback, IProgress<float> progress = null) where T : Object => this.TryLoadAsync(typeof(T).GetKey(), callback, progress);

        public IEnumerator LoadComponentAsync<T>(Action<T> callback, IProgress<float> progress = null) => this.LoadComponentAsync(typeof(T).GetKey(), callback, progress);

        public IEnumerator TryLoadComponentAsync<T>(Action<(bool IsSucceeded, T Component)> callback, IProgress<float> progress = null) => this.TryLoadComponentAsync(typeof(T).GetKey(), callback, progress);

        #endregion

        #endif

        #endregion

        public void Unload(string key);

        public void Unload<T>() => this.Unload(typeof(T).GetKey());
    }
}