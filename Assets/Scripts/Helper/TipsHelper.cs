using LiteMore.Combat.Skill;
using LiteMore.Extend;
using UnityEngine;

namespace LiteMore.Helper
{
    public static class TipsHelper
    {
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

            if (!string.IsNullOrWhiteSpace(Desc.About))
            {
                Builder.Msg("------------------\n")
                .Chunk(Desc.About, Color.white, 20);
            }

            return Builder.GetRichText();
        }
    }
}