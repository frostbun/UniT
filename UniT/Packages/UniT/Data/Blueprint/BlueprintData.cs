namespace UniT.Data.Blueprint
{
    using System.Collections.Generic;

    public abstract class BlueprintData<TKey, TValue> : Dictionary<TKey,TValue>, IBlueprintData
    {
    }
}