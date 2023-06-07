namespace UniT.Data.Player
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class PlayerPrefsJsonDataHandler : PlayerJsonDataHandler
    {
        public override UniTask Flush()
        {
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }

        protected override UniTask<string> GetJson(string key)
        {
            return UniTask.FromResult(PlayerPrefs.GetString(key));
        }

        protected override UniTask SaveJson(string key, string json)
        {
            PlayerPrefs.SetString(key, json);
            return UniTask.CompletedTask;
        }
    }
}