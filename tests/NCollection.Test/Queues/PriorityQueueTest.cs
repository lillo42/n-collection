using System;
using System.Collections.Generic;
using System.Linq;

namespace NCollection.Test.Queues
{
    public class PriorityQueueTest_String : AbstractQueueTest<string>
    {
        protected override string GetPeekValue(string[] array)
        {
            Array.Sort(array, Comparer<string>.Default);
            return array[0];
        }

        protected override AbstractCollection<string> CreateCollection(int size)
        {
            return new PriorityQueue<string>(size);
        }

        protected override AbstractCollection<string> CreateCollection(int size, IEnumerable<string> source)
        {
            return new PriorityQueue<string>(source);
        }

        protected override AbstractQueue<string> CreateQueue()
        {
            return new PriorityQueue<string>();
        }

        protected override AbstractQueue<string> CreateQueue(IEnumerable<string> enumerable)
        {
            return new PriorityQueue<string>(enumerable);
        }
    }
    
    public class PriorityQueueTest_Int : AbstractQueueTest<int>
    {
        protected override int GetPeekValue(int[] array)
        {
            Array.Sort(array, Comparer<int>.Default);
            return array[0];
        }

        protected override AbstractCollection<int> CreateCollection(int size)
        {
            return new PriorityQueue<int>(size);
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> source)
        {
            return new PriorityQueue<int>(source);
        }

        protected override AbstractQueue<int> CreateQueue()
        {
            return new PriorityQueue<int>();
        }

        protected override AbstractQueue<int> CreateQueue(IEnumerable<int> enumerable)
        {
            return new PriorityQueue<int>(enumerable);
        }
    }
    
    public class PriorityQueueTest_Int_Reverse : AbstractQueueTest<int>
    {
        protected override int GetPeekValue(int[] array)
        {
            Array.Sort(array,new Reverse());
            return array[0];
        }

        protected override AbstractCollection<int> CreateCollection(int size)
        {
            return new PriorityQueue<int>(size, new Reverse());
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> source)
        {
            return new PriorityQueue<int>(source, new Reverse());
        }

        protected override AbstractQueue<int> CreateQueue()
        {
            return new PriorityQueue<int>(new Reverse());
        }

        protected override AbstractQueue<int> CreateQueue(IEnumerable<int> enumerable)
        {
            return new PriorityQueue<int>(enumerable, new Reverse());
        }
        
        public class Reverse : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x.CompareTo(y) * -1;
            }
        }
    }
}