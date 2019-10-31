using LiteFramework.Game.Asset;

namespace LiteFramework.Game.UI
{
    public class UIDescriptor
    {
        public AssetUri Uri { get; }
        public bool OpenMore { get; }
        public bool Cached { get; }

        public UIDescriptor(AssetUri Uri, bool OpenMore, bool Cached)
        {
            this.Uri = Uri;
            this.OpenMore = OpenMore;
            this.Cached = Cached;
        }
    }
}