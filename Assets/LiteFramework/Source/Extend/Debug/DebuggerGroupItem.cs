using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LiteFramework.Extend.Debug
{
    internal class DebugGroupItem : BaseDebuggerDrawItem
    {
        private readonly Dictionary<string, IDebuggerDrawItem> ItemList_;
        private int ItemIndex_;
        private IDebuggerDrawItem PreItem_;
        private IDebuggerDrawItem CurItem_;

        public DebugGroupItem(string Name)
        {
            this.Name = Name;
            this.ItemList_ = new Dictionary<string, IDebuggerDrawItem>();
            this.ItemIndex_ = 0;
            this.PreItem_ = null;
            this.CurItem_ = null;
        }

        public override void Enter()
        {
            this.ItemIndex_ = 0;
            this.PreItem_ = null;
            this.CurItem_ = null;
        }

        public override void Exit()
        {
            foreach (var Item in ItemList_)
            {
                Item.Value?.Exit();
            }
        }

        public override void Dispose()
        {
            foreach (var Item in ItemList_)
            {
                Item.Value?.Dispose();
            }
        }

        public override void Draw()
        {
            PreItem_ = CurItem_;
            var Names = ItemList_.Keys.ToArray();
            ItemIndex_ = GUILayout.Toolbar(ItemIndex_, Names, GUILayout.Height(30f), GUILayout.MaxWidth(Screen.width));
            CurItem_ = ItemList_[Names[ItemIndex_]];

            if (PreItem_ != CurItem_)
            {
                PreItem_?.Exit();
                CurItem_?.Enter();
            }

            CurItem_?.Draw();
        }

        // I
        // A/B1
        // A/B2
        // A/B1/C1
        public void Register(string Path, IDebuggerDrawItem Item)
        {
            var Paths = Path.Split('/');
            if (Paths.Length == 1)
            {
                Item.Name = Path;
                ItemList_.Add(Path, Item);
                return;
            }

            if (!ItemList_.ContainsKey(Paths[0]))
            {
                var GroupItem = new DebugGroupItem(Paths[0]);
                GroupItem.Register(Path.Substring(Paths[0].Length + 1), Item);
                ItemList_.Add(Paths[0], GroupItem);
            }
            else if (ItemList_[Paths[0]] is DebugGroupItem DItem)
            {
                DItem.Register(Path.Substring(Paths[0].Length + 1), Item);
            }
            else
            {
                var GroupItem = new DebugGroupItem(Paths[0]);
                GroupItem.Register(ItemList_[Paths[0]].Name, ItemList_[Paths[0]]);
                GroupItem.Register(Path.Substring(Paths[0].Length + 1), Item);
                ItemList_[Paths[0]] = GroupItem;
            }
        }

        public void UnRegister(string Path)
        {
            var Paths = Path.Split('/');
            if (Paths.Length == 1)
            {
                ItemList_.Remove(Path);
                return;
            }

            if (ItemList_.ContainsKey(Paths[0]))
            {
                if (ItemList_[Paths[0]] is DebugGroupItem DItem)
                {
                    DItem.UnRegister(Path.Substring(Paths[0].Length + 1));
                    if (DItem.ItemList_.Count == 0)
                    {
                        ItemList_.Remove(Paths[0]);
                    }
                }
            }
        }
    }
}