#nullable enable
namespace UniT.Models
{
    using UniT.Data.Serialization;
    using UniT.Data.Storage;

    public interface IProgression : IJsonData, IReadableData, IWritableData
    {
    }
}