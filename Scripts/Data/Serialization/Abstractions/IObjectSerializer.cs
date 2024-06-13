#nullable enable
namespace UniT.Data.Serialization
{
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #else
    using System;
    using System.Collections;
    #endif

    public interface IObjectSerializer : ISerializer
    {
        public void Populate(IData data, object rawData);

        public object Serialize(IData data);

        #if UNIT_UNITASK
        public UniTask PopulateAsync(IData data, object rawData);

        public UniTask<object> SerializeAsync(IData data);
        #else
        public IEnumerator PopulateAsync(IData data, object rawData, Action? callback = null);

        public IEnumerator SerializeAsync(IData data, Action<object> callback);
        #endif
    }
}