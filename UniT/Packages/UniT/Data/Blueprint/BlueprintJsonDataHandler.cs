namespace UniT.Data.Blueprint
{
    using System;
    using UniT.Data.Base;

    public abstract class BlueprintJsonDataHandler : BaseJsonDataHandler
    {
        public override bool CanHandle(Type type)
        {
            return typeof(IBlueprintData).IsAssignableFrom(type);
        }
    }
}