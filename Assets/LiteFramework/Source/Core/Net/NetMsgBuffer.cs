namespace LiteFramework.Core.Net
{
    internal class NetMsgBuffer
    {
        public byte[] Data { get; }

        public NetMsgBuffer(byte[] Data)
        {
            this.Data = Data;
        }
    }
}