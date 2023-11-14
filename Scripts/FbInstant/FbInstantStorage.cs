#if UNIT_FBINSTANT
namespace UniT.Data.Storages
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.FbInstant;
    using UnityEngine.Scripting;

    public sealed class FbInstantStorage : IStorage
    {
        [Preserve]
        public FbInstantStorage()
        {
        }

        public bool CanStore(Type type)
        {
            return typeof(IData).IsAssignableFrom(type);
        }

        public UniTask<string[]> Load(string[] keys)
        {
            return FbInstant.Player.LoadData(keys).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Load {keys.ToJson()} error: {result.Error}");
                return result.Data;
            });
        }

        public UniTask Save(string[] keys, string[] rawDatas)
        {
            return FbInstant.Player.SaveData(keys, rawDatas).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Save {keys.ToJson()} error: {result.Error}");
            });
        }

        public UniTask Flush()
        {
            return FbInstant.Player.FlushData().ContinueWith(result =>
            {
                if (result.IsError) throw new($"Flush error: {result.Error}");
            });
        }
    }
}
#endif