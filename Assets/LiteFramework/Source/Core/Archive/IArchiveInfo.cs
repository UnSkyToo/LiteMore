namespace LiteFramework.Core.Archive
{
    public interface IArchiveInfo
    {
        void Encode(ArchiveEncoder Encoder);
        void Decode(ArchiveDecoder Decoder);
    }
}