using System;
using LiteFramework.Core.Async.Timer;
using LiteFramework.Game.UI;
using LiteFramework.Helper;
using LiteMore.Combat.Skill;
using LiteMore.Extend;
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
            TimerEntity Timer = null;
            var BeginPos = Vector2.zero;

            UIHelper.AddEvent(Obj, (_, Pos) =>
            {
                var Msg = GetFunc?.Invoke() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(Msg))
                {
                    return;
                }

                BeginPos = Pos;
                Timer = TimerManager.AddTimer(Configure.TipsHoldTime, () => { UIManager.OpenUI<TipsUI>(Msg, Pos); }, 1);
            }, UIEventType.Down);

            UIHelper.AddEvent(Obj, (_, Pos) =>
            {
                if (Timer == null)
                {
                    return;
                }

                if (Vector2.Distance(Pos, BeginPos) > 10)
                {
                    TimerManager.StopTimer(Timer);
                    Timer = null;
                }
            }, UIEventType.Drag);

            UIHelper.AddEvent(Obj, (_, Pos) =>
            {
                TimerManager.StopTimer(Timer);
                Timer = null;
                UIManager.CloseUI<TipsUI>();
            }, UIEventType.Up);

            UIHelper.AddEvent(Obj, (_, Pos) =>
            {
                TimerManager.StopTimer(Timer);
                Timer = null;
            }, UIEventType.Cancel);
        }

        public static string Skill(SkillDescriptor Desc)
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

            /*if (!string.IsNullOrWhiteSpace(Desc.About))
            {
                Builder.Msg("------------------\n")
                    .Chunk(Desc.About, Color.white, 20);
            }*/

            return Builder.GetRichText();
        }
    }
}