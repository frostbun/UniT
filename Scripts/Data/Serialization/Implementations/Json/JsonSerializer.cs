#if UNIT_JSON
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    using UniT.Extensions;
    #endif

    public sealed class JsonSerializer : IStringSerializer
    {
        private readonly JsonSerializerSettings settings;

        [Preserve]
        public JsonSerializer(JsonSerializerSettings? settings = null)
        {
            this.settings = settings
                ?? new JsonSerializerSettings
                {
                    Culture                = CultureInfo.InvariantCulture,
                    TypeNameHandling       = TypeNameHandling.Auto,
                    ReferenceLoopHandling  = ReferenceLoopHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                };
        }

        bool ISerializer.CanSerialize(Type type) => typeof(IJsonData).IsAssignableFrom(type);

        void IStringSerializer.Populate(IData data, string rawData) => this.Populate(data, rawData);

        string IStringSerializer.Serialize(IData data) => this.Serialize(data);

        #if UNIT_UNITASK
        UniTask IStringSerializer.PopulateAsync(IData data, string rawData) => UniTask.RunOnThreadPool(() => this.Populate(data, rawData));

        UniTask<string> IStringSerializer.SerializeAsync(IData data) => UniTask.RunOnThreadPool(() => this.Serialize(data));
        #else
        IEnumerator IStringSerializer.PopulateAsync(IData data, string rawData, Action? callback) => CoroutineRunner.Run(() => this.Populate(data, rawData), callback);

        IEnumerator IStringSerializer.SerializeAsync(IData data, Action<string> callback) => CoroutineRunner.Run(() => this.Serialize(data), callback);
        #endif

        private void Populate(IData data, string rawData)
        {
            JsonConvert.PopulateObject(rawData, data, this.settings);
        }

        private string Serialize(IData data)
        {
            return JsonConvert.SerializeObject(data, this.settings);
        }
    }
}
#endif