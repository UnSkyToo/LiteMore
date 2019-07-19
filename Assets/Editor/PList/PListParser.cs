using System.Collections.Generic;
using UnityEngine;

namespace Lite.Editor.PList
{
    public class PListDataInfo
    {
        public string Key_;
        public string Value_;
        public string Type_;

        public override string ToString()
        {
            return $"{Key_} : [{Type_}]{Value_}";
        }
    }

    public class PListDataFrameInfo
    {
        public Rect Frame { get; set; }
        public Vector2 Offset { get; set; }
        public bool Rotated { get; set; }
        public Rect SourceColorRect { get; set; }
        public Vector2 SourceSize { get; set; }
    }

    public class PListDataFrame
    {
        public string Key_;
        public PListDataFrameInfo Value_;

        public PListDataFrame()
        {
            Key_ = string.Empty;
            Value_ = new PListDataFrameInfo();
        }
    }

    public class PListParser
    {
        private readonly Dictionary<string, PListDataFrameInfo> Frames_ = new Dictionary<string, PListDataFrameInfo>();
        private Dictionary<string, PListDataInfo> Metadata_ = new Dictionary<string, PListDataInfo>();

        public Dictionary<string, PListDataFrameInfo> Frames => Frames_;
        public Dictionary<string, PListDataInfo> Metadata => Metadata_;

        public string RealTextureName
        {
            get
            {
                if (Metadata_ != null &&  Metadata_.ContainsKey("realTextureFileName"))
                {
                    return Metadata_["realTextureFileName"].Value_;
                }

                return string.Empty;
            }
        }

        public Vector2 TextureSize
        {
            get
            {
                if (Metadata_ != null && Metadata_.ContainsKey("size"))
                {
                    return ParseVec(Metadata_["size"].Value_);
                }

                return Vector2.zero;
            }
        }

        private readonly PListLexer Lexer_;

        public PListParser()
        {
            Lexer_ = new PListLexer();
        }

        public bool Parse(string FilePath)
        {
            if (!Lexer_.LoadFile(FilePath))
            {
                return false;
            }

            return ParseContent();
        }

        private bool ParseContent()
        {
            Frames_.Clear();

            if (!ParseHead())
            {
                return false;
            }

            return ParseDict();
        }

        private bool ParseHead()
        {
            if (!ParseXmlVersionEncodingInfo())
            {
                return false;
            }
            if (!ParsePublicInfo())
            {
                return false;
            }
            if (!ParsePListVersion())
            {
                return false;
            }

            return true;
        }

        private bool ParseXmlVersionEncodingInfo()
        {
            if (!Lexer_.ReadToken(PListTokenType.BracketsLeft))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.QuestionMark))
            {
                return false;
            }
            if (!Lexer_.ReadToken("xml"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Blank))
            {
                return false;
            }
            if (!Lexer_.ReadToken("version"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Equal))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.String))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Blank))
            {
                return false;
            }
            if (!Lexer_.ReadToken("encoding"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Equal))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.String))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.QuestionMark))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.BracketsRight))
            {
                return false;
            }

            return true;
        }

        private bool ParsePublicInfo()
        {
            if (!Lexer_.ReadToken(PListTokenType.BracketsLeft))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.ExclamationMark))
            {
                return false;
            }
            if (!Lexer_.ReadToken("DOCTYPE"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Blank))
            {
                return false;
            }
            if (!Lexer_.ReadToken("plist"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Blank))
            {
                return false;
            }
            if (!Lexer_.ReadToken("PUBLIC"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Blank))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.String))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Blank))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.String))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.BracketsRight))
            {
                return false;
            }

            return true;
        }

        private bool ParsePListVersion()
        {
            if (!Lexer_.ReadToken(PListTokenType.BracketsLeft))
            {
                return false;
            }
            if (!Lexer_.ReadToken("plist"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Blank))
            {
                return false;
            }
            if (!Lexer_.ReadToken("version"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Equal))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.String))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.BracketsRight))
            {
                return false;
            }

            return true;
        }

        private bool CheckKeyToken()
        {
            var CheckToken1 = Lexer_.ReadToken();
            var CheckToken2 = Lexer_.ReadToken();

            Lexer_.RollToken(CheckToken2);
            Lexer_.RollToken(CheckToken1);

            if (CheckToken1.Type_ != PListTokenType.BracketsLeft ||
                CheckToken2.Type_ != PListTokenType.String ||
                CheckToken2.Value_ != "key")
            {
                return false;
            }

            return true;
        }

        private bool ParseDict()
        {
            if (!ReadDictHead())
            {
                return false;
            }

            if (!ReadKeyHead())
            {
                return false;
            }

            if (!Lexer_.ReadToken("frames"))
            {
                return false;
            }

            if (!ReadKeyTail())
            {
                return false;
            }

            if (!ReadDictHead())
            {
                return false;
            }

            while (true)
            {
                if (!CheckKeyToken())
                {
                    break;
                }

                var FrameInfo = ReadFrame();
                if (FrameInfo.Item1 == null || FrameInfo.Item2 == null)
                {
                    return false;
                }

                var Frame = ParserFrame(FrameInfo);
                Frames_.Add(Frame.Key_, Frame.Value_);
            }


            if (!ReadDictTail())
            {
                return false;
            }

            Metadata_ = ReadFrame().Item2;

            if (Metadata_ == null)
            {
                return false;
            }

            return true;
        }

        private (string, Dictionary<string, PListDataInfo>) ReadFrame()
        {
            if (!ReadKeyHead())
            {
                return (null, null);
            }

            var InfoList = new Dictionary<string, PListDataInfo>();

            var Key = Lexer_.ReadToken();
            if (Key.Type_ != PListTokenType.String)
            {
                return (null, null);
            }

            if (!ReadKeyTail())
            {
                return (null, null);
            }

            if (!ReadDictHead())
            {
                return (null, null);
            }

            while (true)
            {
                if (!CheckKeyToken())
                {
                    break;
                }

                var Pairs = ReadInfo();
                if (Pairs == null)
                {
                    return (null, null);
                }

                InfoList.Add(Pairs.Key_, Pairs);
            }


            if (!ReadDictTail())
            {
                return (null, null);
            }

            return (Key.Value_, InfoList);
        }

        private PListDataFrame ParserFrame((string, Dictionary<string, PListDataInfo>) Value)
        {
            var Frame = new PListDataFrame();
            Frame.Key_ = Value.Item1;
            Frame.Value_.Frame = ParseRect(Value.Item2["frame"].Value_);
            Frame.Value_.Offset = ParseVec(Value.Item2["offset"].Value_);
            Frame.Value_.Rotated = bool.Parse(Value.Item2["rotated"].Value_);
            Frame.Value_.SourceColorRect = ParseRect(Value.Item2["sourceColorRect"].Value_);
            Frame.Value_.SourceSize = ParseVec(Value.Item2["sourceSize"].Value_);
            return Frame;
        }

        private PListDataInfo ReadInfo()
        {
            if (!ReadKeyHead())
            {
                return null;
            }

            var Key = Lexer_.ReadToken();
            if (Key.Type_ != PListTokenType.String)
            {
                return null;
            }

            if (!ReadKeyTail())
            {
                return null;
            }

            if (!Lexer_.ReadToken(PListTokenType.BracketsLeft))
            {
                return null;
            }

            var Data = new PListDataInfo();
            Data.Key_ = Key.Value_;

            var TokenType = Lexer_.ReadToken();
            if (TokenType.Type_ != PListTokenType.String)
            {
                return null;
            }

            if (Lexer_.PeekChar() == '/')
            {
                if (!Lexer_.ReadToken(PListTokenType.Backslash))
                {
                    return null;
                }

                if (!Lexer_.ReadToken(PListTokenType.BracketsRight))
                {
                    return null;
                }

                Data.Value_ = TokenType.Value_;
                Data.Type_ = TokenType.Value_;
            }
            else
            {
                if (!Lexer_.ReadToken(PListTokenType.BracketsRight))
                {
                    return null;
                }

                var TokenValue = Lexer_.ReadToken();
                if (TokenValue.Type_ != PListTokenType.String)
                {
                    return null;
                }

                if (!Lexer_.ReadToken(PListTokenType.BracketsLeft))
                {
                    return null;
                }

                if (!Lexer_.ReadToken(PListTokenType.Backslash))
                {
                    return null;
                }

                var TypeCheck = Lexer_.ReadToken();
                if (TypeCheck.Type_ != PListTokenType.String)
                {
                    return null;
                }

                if (TokenType.Value_ != TypeCheck.Value_)
                {
                    return null;
                }

                if (!Lexer_.ReadToken(PListTokenType.BracketsRight))
                {
                    return null;
                }

                Data.Value_ = TokenValue.Value_;
                Data.Type_ = TokenType.Value_;
            }

            return Data;
        }

        // <dict>
        private bool ReadDictHead()
        {
            if (!Lexer_.ReadToken(PListTokenType.BracketsLeft))
            {
                return false;
            }
            if (!Lexer_.ReadToken("dict"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.BracketsRight))
            {
                return false;
            }

            return true;
        }

        //  </dict>
        private bool ReadDictTail()
        {
            if (!Lexer_.ReadToken(PListTokenType.BracketsLeft))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Backslash))
            {
                return false;
            }
            if (!Lexer_.ReadToken("dict"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.BracketsRight))
            {
                return false;
            }

            return true;
        }

        // <key>
        private bool ReadKeyHead()
        {
            if (!Lexer_.ReadToken(PListTokenType.BracketsLeft))
            {
                return false;
            }
            if (!Lexer_.ReadToken("key"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.BracketsRight))
            {
                return false;
            }

            return true;
        }

        // </key>
        private bool ReadKeyTail()
        {
            if (!Lexer_.ReadToken(PListTokenType.BracketsLeft))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.Backslash))
            {
                return false;
            }
            if (!Lexer_.ReadToken("key"))
            {
                return false;
            }
            if (!Lexer_.ReadToken(PListTokenType.BracketsRight))
            {
                return false;
            }

            return true;
        }

        private Rect ParseRect(string Value)
        {
            try
            {
                var Temp = Value.Split(new[] {'{', '}', ','});
                var Params = new List<string>();
                foreach (var T in Temp)
                {
                    if (!string.IsNullOrWhiteSpace(T))
                    {
                        Params.Add(T);
                    }
                }

                var FrameRect = new Rect();
                FrameRect.x = float.Parse(Params[0]);
                FrameRect.y = float.Parse(Params[1]);
                FrameRect.width = float.Parse(Params[2]);
                FrameRect.height = float.Parse(Params[3]);
                return FrameRect;
            }
            catch
            {
                return Rect.zero;
            }
        }

        private Vector2 ParseVec(string Value)
        {
            try
            {
                var Temp = Value.Split(new[] { '{', '}', ',' });
                var Params = new List<string>();
                foreach (var T in Temp)
                {
                    if (!string.IsNullOrWhiteSpace(T))
                    {
                        Params.Add(T);
                    }
                }

                return new Vector2(float.Parse(Params[0]), float.Parse(Params[1]));
            }
            catch
            {
                return Vector2.zero;
            }
        }
    }
}