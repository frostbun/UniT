namespace UniT.Data.Player
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class LocalPlayerJsonDataHandler : PlayerJsonDataHandler
    {
        public override UniTask Flush()
        {
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }

        protected override UniTask<string> GetJson(Type type)
        {
            return UniTask.FromResult(PlayerPrefs.GetString(type.Name));
        }

        protected override UniTask         SaveJson(string json, Type type)
        {
            PlayerPrefs.SetString(type.Name, json);
            return UniTask.CompletedTask;
        }
    }
}