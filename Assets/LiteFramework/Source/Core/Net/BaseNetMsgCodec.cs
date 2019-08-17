namespace LiteFramework.Core.Net
{
    public abstract class BaseNetMsgCodec
    {
        public abstract byte[] Encode<T>(T NetMsg) where T : BaseNetMsg;
        public abstract object Decode(byte[] Buffer);
    }
}