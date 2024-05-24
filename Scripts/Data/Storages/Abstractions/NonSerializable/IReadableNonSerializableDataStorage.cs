﻿#nullable enable
namespace UniT.Data
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IReadableNonSerializableDataStorage : IReadableDataStorage
    {
        public IData[] Read(string[] keys);

        #if UNIT_UNITASK
        public UniTask<IData[]> ReadAsync(string[] keys, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator ReadAsync(string[] keys, Action<IData[]> callback, IProgress<float>? progress = null);
        #endif
    }
}