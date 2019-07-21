using System.Text;
using UnityEngine;

namespace LiteMore.Extend
{
    public class RichTextChunk
    {
        public string Text { get; }

        public RichTextChunk(string Msg, Color TextColor)
        {
            Text = $"<color=#{ColorUtility.ToHtmlStringRGBA(TextColor)}>{Msg}</color>";
        }

        public RichTextChunk(string Msg, Color TextColor, int Size)
        {
            Text = $"<color=#{ColorUtility.ToHtmlStringRGBA(TextColor)}><size={Size}>{Msg}</size></color>";
        }

        public RichTextChunk(string Msg, Color TextColor, int Size, bool Bold, bool Italic)
        {
            Text = $"<color=#{ColorUtility.ToHtmlStringRGBA(TextColor)}><size={Size}>{Msg}</size></color>";
            if (Bold)
            {
                Text = $"<b>{Text}</b>";
            }

            if (Italic)
            {
                Text = $"<i>{Text}</i>";
            }
        }
    }

    public class RichTextBuilder
    {
        private readonly StringBuilder Text_;

        public RichTextBuilder()
        {
            Text_ = new StringBuilder();
        }

        public string GetRichText()
        {
            return Text_.ToString();
        }

        public RichTextBuilder Msg(string Msg)
        {
            Text_.Append(Msg);
            return this;
        }

        public RichTextBuilder Chunk(RichTextChunk Chunk)
        {
            Text_.Append(Chunk.Text);
            return this;
        }

        public RichTextBuilder Chunk(string Msg, Color TextColor)
        {
            Chunk(new RichTextChunk(Msg, TextColor));
            return this;
        }

        public RichTextBuilder Chunk(string Msg, Color TextColor, int Size)
        {
            Chunk(new RichTextChunk(Msg, TextColor, Size));
            return this;
        }

        public RichTextBuilder Chunk(string Msg, Color TextColor, int Size, bool Bold, bool Italic)
        {
            Chunk(new RichTextChunk(Msg, TextColor, Size, Bold, Italic));
            return this;
        }

        public RichTextBuilder NewLine()
        {
            Text_.AppendLine();
            return this;
        }

        public RichTextBuilder ColorL(Color TextColor)
        {
            Text_.Append($"<color=#{ColorUtility.ToHtmlStringRGBA(TextColor)}>");
            return this;
        }

        public RichTextBuilder ColorR()
        {
            Text_.Append("</color>");
            return this;
        }

        public RichTextBuilder SizeL(int Size)
        {
            Text_.Append($"<size={Size}>");
            return this;
        }

        public RichTextBuilder SizeR()
        {
            Text_.Append("</size>");
            return this;
        }

        public RichTextBuilder BoldL()
        {
            Text_.Append("<b>");
            return this;
        }

        public RichTextBuilder BoldR()
        {
            Text_.Append("</b>");
            return this;
        }

        public RichTextBuilder ItalicL()
        {
            Text_.Append("<i>");
            return this;
        }

        public RichTextBuilder ItalicR()
        {
            Text_.Append("</i>");
            return this;
        }
    }
}