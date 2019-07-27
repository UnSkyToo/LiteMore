using System;
using LiteMore.Combat.Skill;
using LiteMore.Extend;
using LiteMore.UI;
using LiteMore.UI.Core;
using UnityEngine;

namespace LiteMore.Helper
{
    public static class TipsHelper
    {
        public static void AddTips(Transform Parent, string ChildPath, Func<string> GetFunc)
        {
            var Child = UIHelper.FindChild(Parent, ChildPath);
            if (Child == null)
            {
                return;
            }

            AddTips(Child, GetFunc);
        }

        public static void AddTips(Transform Obj, Func<string> GetFunc)
        {
            var TimerID = 0u;
            var BeginPos = Vector2.zero;
            BaseUI Tips = null;

            UIHelper.AddEvent(Obj, (_, Pos) =>
            {
                var Msg = GetFunc?.Invoke() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(Msg))
                {
                    return;
                }

                BeginPos = Pos;
                TimerID = TimerManager.AddTimer(Configure.TipsHoldTime, () => { Tips = UIManager.OpenUI<TipsUI>(Msg, Pos); }, 1);
            }, UIEventType.Down);

            UIHelper.AddEvent(Obj, (_, Pos) =>
            {
                if (TimerID == 0)
                {
                    return;
                }

                if (Vector2.Distance(Pos, BeginPos) > 10)
                {
                    TimerManager.StopTimer(TimerID);
                    TimerID = 0;
                }
            }, UIEventType.Drag);

            UIHelper.AddEvent(Obj, (_, Pos) =>
            {
                TimerManager.StopTimer(TimerID);
                TimerID = 0;
                UIManager.CloseUI(Tips);
            }, UIEventType.Up);

            UIHelper.AddEvent(Obj, (_, Pos) =>
            {
                TimerManager.StopTimer(TimerID);
                TimerID = 0;
            }, UIEventType.Cancel);
        }

        public static string Skill(BaseSkillDescriptor Desc)
        {
            var Builder = new RichTextBuilder();
            Builder
                .Chunk($"{Desc.Name}\n", Color.red)
                .Msg("------------------\n")
                .Chunk($"冷却:{Desc.CD}s\n", Color.green)
                .Chunk($"消耗:{Desc.Cost}\n", Color.blue);

            if (Desc.Radius > 0)
            {
                Builder.Chunk($"半径:{Desc.Radius}\n", Color.blue);
            }

            if (!string.IsNullOrWhiteSpace(Desc.About))
            {
                Builder.Msg("------------------\n")
                    .Chunk(Desc.About, Color.white, 20);
            }

            return Builder.GetRichText();
        }
    }
}