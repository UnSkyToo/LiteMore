using System;
using System.Collections.Generic;

namespace LiteMore.Combat.Skill
{
    public enum QuickCode : byte
    {
        Metal = 0,
        Wood = 1,
        Water = 2,
        Fire = 3,
        Earth = 4,
    }

    public struct QuickIndex
    {
        private readonly byte Metal_;
        private readonly byte Wood_;
        private readonly byte Water_;
        private readonly byte Fire_;
        private readonly byte Earth_;

        public QuickIndex(byte Metal, byte Wood, byte Water, byte Fire, byte Earth)
        {
            this.Metal_ = Metal;
            this.Wood_ = Wood;
            this.Water_ = Water;
            this.Fire_ = Fire;
            this.Earth_ = Earth;
        }

        public override string ToString()
        {
            return $"<Metal({Metal_}), Wood({Wood_}), Water({Water_}), Fire({Fire_}), Earth({Earth_})>";
        }

        public override bool Equals(object Obj)
        {
            if (Obj == null)
            {
                return false;
            }

            if (Obj.GetType() != typeof(QuickIndex))
            {
                return false;
            }

            var Qe = (QuickIndex)Obj;
            return Equals(this, Qe);
        }

        public bool Equals(QuickIndex Other)
        {
            return Metal_ == Other.Metal_ &&
                   Wood_  == Other.Wood_  &&
                   Water_ == Other.Water_ &&
                   Fire_  == Other.Fire_  &&
                   Earth_ == Other.Earth_;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(QuickIndex Left, QuickIndex Right)
        {
            return Left.Equals(Right);
        }

        public static bool operator !=(QuickIndex Left, QuickIndex Right)
        {
            return !Left.Equals(Right);
        }
    }

    public class QuickNode
    {
        public uint ID { get; }

        public QuickNode(uint ID)
        {
            this.ID = ID;
        }
    }

    public class QuickController
    {
        public const int MaxComposeElementCount = 5;
        public const float ControlTime = 0.3f;

        public event Action<QuickNode> OnProbe;
        public event Action<QuickNode> OnSucceed;
        public event Action OnFailed; 

        private readonly Dictionary<int, QuickNode> Elements_;

        private bool IsHandle_;
        private float Time_;

        private byte Metal_;
        private byte Wood_;
        private byte Water_;
        private byte Fire_;
        private byte Earth_;

        public QuickController()
        {
            Elements_ = new Dictionary<int, QuickNode>();
            Reset();
        }

        public void Tick(float DeltaTime)
        {
            if (!IsHandle_)
            {
                return;
            }

            Time_ += DeltaTime;
            if (Time_ >= ControlTime)
            {
                Done();
            }
        }

        public void ExecuteCode(QuickCode Code)
        {
            IsHandle_ = true;
            Time_ = 0;

            switch (Code)
            {
                case QuickCode.Metal:
                    Metal_++;
                    break;
                case QuickCode.Wood:
                    Wood_++;
                    break;
                case QuickCode.Water:
                    Water_++;
                    break;
                case QuickCode.Fire:
                    Fire_++;
                    break;
                case QuickCode.Earth:
                    Earth_++;
                    break;
                default:
                    break;
            }

            var Total = Metal_ + Wood_ + Water_ + Fire_ + Earth_;
            if (Total >= MaxComposeElementCount)
            {
                Done();
            }
            else
            {
                CheckProbe(false);
            }
        }

        public void AddNode(QuickIndex Index, QuickNode Node)
        {
            if (Elements_.ContainsKey(Index.GetHashCode()))
            {
                UnityEngine.Debug.LogWarning($"Repeat QuickNode {Node.ID}");
                return;
            }
            Elements_.Add(Index.GetHashCode(), Node);
        }

        private void Done()
        {
            CheckProbe(true);
            Reset();
        }

        private void Reset()
        {
            IsHandle_ = false;
            Time_ = 0;

            Metal_ = 0;
            Wood_ = 0;
            Water_ = 0;
            Fire_ = 0;
            Earth_ = 0;
        }

        private void CheckProbe(bool IsFinal)
        {
            var Index = new QuickIndex(Metal_, Wood_, Water_, Fire_, Earth_);
            
            if (Elements_.ContainsKey(Index.GetHashCode()))
            {
                var Node = Elements_[Index.GetHashCode()];
                if (IsFinal)
                {
                    OnSucceed?.Invoke(Node);
                }
                else
                {
                    OnProbe?.Invoke(Node);
                }
            }
            else
            {
                if (IsFinal)
                {
                    OnFailed?.Invoke();
                }
            }
        }
    }
}