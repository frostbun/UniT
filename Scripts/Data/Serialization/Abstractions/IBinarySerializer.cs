#nullable enable
namespace UniT.Data.Serialization
{
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #else
    using System;
    using System.Collections;
    #endif

    public interface IBinarySerializer : ISerializer
    {
        public void Populate(IData data, byte[] rawData);

        public byte[] Serialize(IData data);

        #if UNIT_UNITASK
        public UniTask PopulateAsync(IData data, byte[] rawData);

        public UniTask<byte[]> SerializeAsync(IData data);
        #else
        public IEnumerator PopulateAsync(IData data, byte[] rawData, Action? callback = null);

        public IEnumerator SerializeAsync(IData data, Action<byte[]> callback);
        #endif
    }
}