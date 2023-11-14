#if UNIT_FBINSTANT
namespace UniT.Data.Storages
{
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.FbInstant;
    using UnityEngine.Scripting;

    public sealed class FbInstantStorage : BaseStorage
    {
        [Preserve]
        public FbInstantStorage()
        {
        }

        protected override UniTask<string[]> Load(string[] keys)
        {
            return FbInstant.Player.LoadData(keys).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Load {keys.ToJson()} error: {result.Error}");
                return result.Data;
            });
        }

        protected override UniTask Save(string[] keys, string[] rawDatas)
        {
            return FbInstant.Player.SaveData(keys, rawDatas).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Save {keys.ToJson()} error: {result.Error}");
            });
        }

        protected override UniTask Flush()
        {
            return FbInstant.Player.FlushData().ContinueWith(result =>
            {
                if (result.IsError) throw new($"Flush {this.GetType().Name} error: {result.Error}");
            });
        }
    }
}
#endif