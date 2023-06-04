namespace UniT.Data.Blueprint
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Data.Base;

    public abstract class BlueprintJsonDataHandler : BaseJsonDataHandler
    {
        public sealed override UniTask Flush()
        {
            return UniTask.CompletedTask;
        }

        public override bool CanHandle(Type type)
        {
            return typeof(IBlueprintData).IsAssignableFrom(type);
        }

        protected sealed override UniTask SaveJson(string json, Type type)
        {
            return UniTask.CompletedTask;
        }
    }
}