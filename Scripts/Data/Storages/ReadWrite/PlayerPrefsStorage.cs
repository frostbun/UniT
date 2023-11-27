namespace UniT.Data.Storages
{
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public sealed class PlayerPrefsStorage : ReadWriteStorage
    {
        [Preserve]
        public PlayerPrefsStorage()
        {
        }

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
        #endif
    }
}