using System.Collections;
using System.Collections.Generic;

namespace LiteMore.Extend
{
    public class ListEx<T> : IEnumerable<T>
    {
        private bool Dirty_;

        private readonly List<T> Values_;
        private readonly List<T> AddList_;
        private readonly List<T> RemoveList_;

        public ListEx()
        {
            Dirty_ = false;

            Values_ = new List<T>();
            AddList_ = new List<T>();
            RemoveList_ = new List<T>();
        }

        public void Add(T Item)
        {
            AddList_.Add(Item);
            Dirty_ = true;
        }

        public void Remove(T Item)
        {
            RemoveList_.Add(Item);
            Dirty_ = true;
        }

        public void Clear()
        {
            RemoveList_.Clear();
            AddList_.Clear();
            Values_.Clear();
        }

        public void Flush()
        {
            if (Dirty_)
            {
                foreach (var Item in RemoveList_)
                {
                    Values_.Remove(Item);
                }

                RemoveList_.Clear();

                foreach (var Item in AddList_)
                {
                    Values_.Add(Item);
                }

                AddList_.Clear();
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (var Item in Values_)
            {
                yield return Item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var Item in Values_)
            {
                yield return Item;
            }
        }
    }
}