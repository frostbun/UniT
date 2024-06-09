﻿#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using UniT.Extensions;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    using System.Threading.Tasks;
    #endif

    public sealed class ObjectSerializer : IObjectSerializer
    {
        [Preserve]
        public ObjectSerializer()
        {
        }

        bool ISerializer.CanSerialize(Type type) => typeof(object).IsAssignableFrom(type);

        void IObjectSerializer.Populate(IData data, object rawData) => Populate(data, rawData);

        object IObjectSerializer.Serialize(IData data) => Serialize(data);

        #if UNIT_UNITASK
        UniTask IObjectSerializer.PopulateAsync(IData data, object rawData) => UniTask.RunOnThreadPool(() => Populate(data, rawData));

        UniTask<object> IObjectSerializer.SerializeAsync(IData data) => UniTask.RunOnThreadPool(() => Serialize(data));
        #else
        IEnumerator IObjectSerializer.PopulateAsync(IData data, object rawData, Action callback) => Task.Run(() => Populate(data, rawData)).ToCoroutine(callback);

        IEnumerator IObjectSerializer.SerializeAsync(IData data, Action<object> callback) => Task.Run(() => Serialize(data)).ToCoroutine(callback);
        #endif

        private static void Populate(IData data, object rawData) => rawData.CopyTo(data);

        private static object Serialize(IData data) => data;
    }
}