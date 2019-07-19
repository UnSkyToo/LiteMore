using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lite.Editor.PList
{
    public enum PListTokenType
    {
        None,               // 未知
        BracketsLeft,       // 左括号
        BracketsRight,      // 右括号
        Blank,              // 空格
        String,             // 字符
        Equal,              // 等号
        QuestionMark,       // 问号
        ExclamationMark,    // 叹号
        QuotationMark,      // 引号
        Backslash,          // 反斜杠

        EndOfFile,          // 文件结束标记
    }

    public class PListToken
    {
        public PListTokenType Type_;
        public string Value_;

        public bool IsNone => Type_ == PListTokenType.None;

        public PListToken()
        {
            Type_ = PListTokenType.None;
            Value_ = string.Empty;
        }
    }

    public class PListLexer
    {
        private readonly List<string> Lines_ = new List<string>();
        private int LineIndex_ = 0;
        private int CharIndex_ = 0;

        public PListLexer()
        {
        }

        public bool LoadFile(string FilePath)
        {
            try
            {
                Lines_.Clear();
                LineIndex_ = 0;
                CharIndex_ = 0;

                var InStream = new StreamReader(FilePath, Encoding.UTF8);
                while (!InStream.EndOfStream)
                {
                    var Line = InStream.ReadLine();
                    if (!string.IsNullOrWhiteSpace(Line))
                    {
                        Line = Line.TrimStart(' ');
                        Line = Line.TrimEnd(' ');
                        Lines_.Add(Line);
                    }
                }

                InStream.Close();
                InStream.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private char ReadChar()
        {
            if (CharIndex_ < Lines_[LineIndex_].Length)
            {
                return Lines_[LineIndex_][CharIndex_++];
            }

            CharIndex_ = 0;
            LineIndex_++;

            if (LineIndex_ >= Lines_.Count)
            {
                return char.MinValue;
            }

            return Lines_[LineIndex_][CharIndex_++];
        }

        public char PeekChar()
        {
            if (CharIndex_ < Lines_[LineIndex_].Length)
            {
                return Lines_[LineIndex_][CharIndex_];
            }

            if (LineIndex_ + 1 >= Lines_.Count)
            {
                return char.MinValue;
            }

            return Lines_[LineIndex_ + 1][0];
        }

        private void RollChar()
        {
            if (CharIndex_ > 0)
            {
                CharIndex_--;
            }
            else
            {
                if (LineIndex_ > 0)
                {
                    LineIndex_--;
                }
                else
                {
                    LineIndex_ = 0;
                }

                CharIndex_ = Lines_[LineIndex_].Length - 1;
            }
        }

        private bool IsSymbol(char Ch)
        {
            switch (Ch)
            {
                case '<':
                case '>':
                case '?':
                case '!':
                case '/':
                case ' ':
                case '=':
                case '"':
                    return true;
                default:
                    return false;
            }
        }

        public PListToken ReadToken()
        {
            var Token = new PListToken();
            var Text = new StringBuilder();
            var InString = false;

            while (true)
            {
                var Ch = ReadChar();

                if (Ch == char.MinValue)
                {
                    Token.Type_ = PListTokenType.EndOfFile;
                    break;
                }

                if (Ch == '"' && !InString)
                {
                    InString = true;
                    continue;
                }

                if (InString || !IsSymbol(Ch))
                {
                    if (Ch == '"')
                    {
                        InString = false;
                        break;
                    }

                    Text.Append(Ch);
                }
                else
                {
                    if (Text.Length == 0)
                    {
                        Text.Append(Ch);
                    }
                    else
                    {
                        RollChar();
                    }
                    break;
                }
            }

            Token.Value_ = Text.ToString();
            Token.Type_ = GetType(Token.Value_);

            return Token;
        }

        public bool ReadToken(PListTokenType TokenType)
        {
            var Token = ReadToken();
            if (Token.Type_ != TokenType)
            {
                return false;
            }

            return true;
        }

        public bool ReadToken(string Value)
        {
            var Token = ReadToken();
            if (Token.Type_ != PListTokenType.String || Token.Value_ != Value)
            {
                return false;
            }

            return true;
        }

        public void RollToken(PListToken Token)
        {
            var Len = Token.Value_.Length;
            for (var Index = 0; Index < Len; ++Index)
            {
                RollChar();
            }
        }

        private PListTokenType GetType(string Value)
        {
            if (string.IsNullOrEmpty(Value))
            {
                return PListTokenType.None;
            }

            switch (Value)
            {
                case "<":
                    return PListTokenType.BracketsLeft;
                case ">":
                    return PListTokenType.BracketsRight;
                case " ":
                    return PListTokenType.Blank;
                case "=":
                    return PListTokenType.Equal;
                case "?":
                    return PListTokenType.QuestionMark;
                case "!":
                    return PListTokenType.ExclamationMark;
                case "/":
                    return PListTokenType.Backslash;
                default:
                    return PListTokenType.String;
            }
        }
    }
}