using LiteFramework.Helper;

namespace LiteFramework.Game.Asset
{
    public sealed class AssetUri
    {
        public string AssetPath { get; }
        public string AssetName { get; }

        public AssetUri(string AssetPath, string AssetName)
        {
            this.AssetPath = AssetPath.ToLower();
            this.AssetName = AssetName.ToLower();
        }

        public AssetUri(string AssetPath)
            : this(AssetPath, PathHelper.GetFileNameWithoutExt(AssetPath))
        {
        }

        public override string ToString()
        {
            return $"{AssetPath}({AssetName})";
        }

        /*public static implicit operator AssetUri(string Uri)
        {
            return new AssetUri(Uri);
        }*/

        public static implicit operator string(AssetUri Uri)
        {
            return Uri.ToString();
        }
    }
}