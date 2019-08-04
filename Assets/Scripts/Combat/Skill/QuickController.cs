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
        public static readonly QuickIndex Null = new QuickIndex(0, 0, 0, 0, 0);

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

        public QuickIndex(byte[] Elements)
        {
            if (Elements != null && Elements.Length == 5)
            {
                this.Metal_ = Elements[0];
                this.Wood_ = Elements[1];
                this.Water_ = Elements[2];
                this.Fire_ = Elements[3];
                this.Earth_ = Elements[4];
            }
            else
            {
                this.Metal_ = 0;
                this.Wood_ = 0;
                this.Water_ = 0;
                this.Fire_ = 0;
                this.Earth_ = 0;
            }
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
        public static readonly int MaxElementCount = 5;
        public const int MaxComposeElementCount = 5;
        public const float ControlTime = 0.5f;

        public event Action<QuickNode> OnProbe;
        public event Action<QuickNode> OnSucceed;
        public event Action<QuickCode> OnCode; 
        public event Action OnFailed; 

        private readonly Dictionary<int, QuickNode> Elements_;
        private readonly byte[] ElementsCount_;

        private bool IsHandle_;
        private float Time_;


        public QuickController()
        {
            Elements_ = new Dictionary<int, QuickNode>();
            ElementsCount_ = new byte[MaxElementCount];
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

            ElementsCount_[(int)Code]++;

            var Total = 0;
            foreach (var Num in ElementsCount_)
            {
                Total += Num;
            }

            if (Total > MaxComposeElementCount)
            {
                ElementsCount_[(int)Code]--;
                Done();
                ExecuteCode(Code);
            }
            else
            {
                OnCode?.Invoke(Code);
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

            for (var Index = 0; Index < MaxElementCount; ++Index)
            {
                ElementsCount_[Index] = 0;
            }
        }

        private void CheckProbe(bool IsFinal)
        {
            var Index = new QuickIndex(ElementsCount_);
            
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
                else
                {
                    OnProbe?.Invoke(null);
                }
            }
        }
    }
}