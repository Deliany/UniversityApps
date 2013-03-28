using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BidirectionalSearch.Helper
{
    public class PriorityQueue<TValue> : PriorityQueue<TValue, Double> { }

    public class PriorityQueue<TValue, TPriority> where TPriority : IComparable
    {
        private readonly SortedDictionary<TPriority, Queue<TValue>> dict = new SortedDictionary<TPriority, Queue<TValue>>();

        public int Count { get; private set; }
        public bool Empty { get { return Count == 0; } }

        public void Enqueue(TValue val)
        {
            Enqueue(val, default(TPriority));
        }

        public void Enqueue(TValue val, TPriority pri)
        {
            ++Count;
            if (!dict.ContainsKey(pri)) dict[pri] = new Queue<TValue>();
            dict[pri].Enqueue(val);
        }

        public TValue Dequeue()
        {
            --Count;
            var item = dict.First();
            if (item.Value.Count == 1) dict.Remove(item.Key);
            return item.Value.Dequeue();
        }

        public TPriority PriorityAtTop()
        {
            var item = dict.First();
            return item.Key;
        }

        public void Remove(TValue val)
        {
            --Count;
            var item = dict.Single(el => el.Value.Contains(val));
            if (item.Value.Count == 1) dict.Remove(item.Key);
            item.Value.Dequeue();
        }

        public TPriority GetPriority(TValue val)
        {
            var item = dict.Single(el => el.Value.Contains(val));
            return item.Key;
        }

        public bool Contains(TValue val)
        {
            return dict.Any(el => el.Value.Contains(val));
        }

        public void Clear()
        {
            this.dict.Clear();
            this.Count = 0;
        }
    }
}
