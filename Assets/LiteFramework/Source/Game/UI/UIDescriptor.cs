namespace LiteFramework.Game.UI
{
    public class UIDescriptor
    {
        public string PrefabName { get; }
        public bool OpenMore { get; }
        public bool Cached { get; }

        public UIDescriptor(string PrefabName)
            : this(PrefabName, true, true)
        {
        }

        public UIDescriptor(string PrefabName, bool OpenMore, bool Cached)
        {
            this.PrefabName = PrefabName;
            this.OpenMore = OpenMore;
            this.Cached = Cached;
        }
    }
}