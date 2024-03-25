namespace UniT.Data
{
    using System;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    [Preserve]
    public sealed class PlayerPrefsDataStorage : ReadWriteSerializableDataStorage
    {
        protected override string[] Load(string[] keys)
        {
            return keys.Select(PlayerPrefs.GetString).ToArray();
        }

        protected override void Save(string[] keys, string[] values)
        {
            IterTools.StrictZip(keys, values).ForEach(PlayerPrefs.SetString);
        }

        protected override void Flush()
        {
            PlayerPrefs.Save();
        }

        #if UNIT_UNITASK
        protected override UniTask<string[]> LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var rawDatas = this.Load(keys);
            progress?.Report(1);
            return UniTask.FromResult(rawDatas);
        }

        protected override UniTask SaveAsync(string[] keys, string[] values, IProgress<float> progress, CancellationToken cancellationToken)
        {
            this.Save(keys, values);
            progress?.Report(1);
            return UniTask.CompletedTask;
        }

        protected override UniTask FlushAsync(IProgress<float> progress, CancellationToken cancellationToken)
        {
            this.Flush();
            progress?.Report(1);
            return UniTask.CompletedTask;
        }
        #else
        protected override IEnumerator LoadAsync(string[] keys, Action<string[]> callback, IProgress<float> progress)
        {
            var rawDatas = this.Load(keys);
            progress?.Report(1);
            callback(rawDatas);
            yield break;
        }

        protected override IEnumerator SaveAsync(string[] keys, string[] values, Action callback, IProgress<float> progress)
        {
            this.Save(keys, values);
            progress?.Report(1);
            callback?.Invoke();
            yield break;
        }

        protected override IEnumerator FlushAsync(Action callback, IProgress<float> progress)
        {
            this.Flush();
            progress?.Report(1);
            callback?.Invoke();
            yield break;
        }
        #endif
    }
}