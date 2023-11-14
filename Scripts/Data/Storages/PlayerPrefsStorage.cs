namespace UniT.Data.Storages
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.Scripting;

    public sealed class PlayerPrefsStorage : IStorage
    {
        [Preserve]
        public PlayerPrefsStorage()
        {
        }

        public bool CanStore(Type type)
        {
            return typeof(IData).IsAssignableFrom(type);
        }

        public UniTask<string[]> Load(string[] keys)
        {
            return UniTask.FromResult(keys.Select(PlayerPrefs.GetString).ToArray());
        }

        public UniTask Save(string[] keys, string[] values)
        {
            IterTools.Zip(keys, values).ForEach(PlayerPrefs.SetString);
            return UniTask.CompletedTask;
        }

        public UniTask Flush()
        {
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }
    }
}