#nullable enable
namespace UniT.Models
{
    using UniT.Data.Serialization;
    using UniT.Data.Storage;

    public interface IPlayerData : IJsonData, IReadableData, IWritableData
    {
    }
}