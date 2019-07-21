using LiteMore.Combat;
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
                .Chunk($"CD:{Desc.CD}s\n", Color.green)
                .Chunk($"Cost:{Desc.Cost}\n", Color.blue);
            return Builder.GetRichText();
        }
    }
}