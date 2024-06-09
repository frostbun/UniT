#nullable enable
namespace UniT.Data.Serialization
{
    using System;

    public interface ISerializer
    {
        public bool CanSerialize(Type type);
    }
}