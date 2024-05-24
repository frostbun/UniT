#nullable enable
namespace UniT.Data
{
    using System;

    public interface ISerializer
    {
        public bool CanSerialize(Type type);
    }
}