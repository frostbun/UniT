namespace UniT.Core.Data.Blueprint
{
    using System.Collections.Generic;

    public abstract class BlueprintData<TKey, TValue> : Dictionary<TKey,TValue>, IBlueprintData
    {
    }
}