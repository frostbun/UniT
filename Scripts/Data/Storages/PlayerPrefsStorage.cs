namespace UniT.Data.Storages
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.Scripting;

    public sealed class PlayerPrefsStorage : BaseStorage
    {
        [Preserve]
        public PlayerPrefsStorage()
        {
        }

        protected override UniTask<string[]> Load(string[] keys)
        {
            return UniTask.FromResult(keys.Select(PlayerPrefs.GetString).ToArray());
        }

        protected override UniTask Save(string[] keys, string[] values)
        {
            IterTools.Zip(keys, values).ForEach(PlayerPrefs.SetString);
            return UniTask.CompletedTask;
        }

        protected override UniTask Flush()
        {
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }
    }
}