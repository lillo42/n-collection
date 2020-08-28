using System.Collections.Generic;

namespace NCollection.Test.Queues
{
    public class ArrayQueueTest_String : AbstractQueueTest<string>
    {
        protected override AbstractCollection<string> CreateCollection(int size)
        {
            return new ArrayQueue<string>(size);
        }

        protected override AbstractCollection<string> CreateCollection(int size, IEnumerable<string> enumerable)
        {
            return new ArrayQueue<string>(size, enumerable);
        }

        protected override AbstractQueue<string> CreateQueue()
        {
            return new ArrayQueue<string>();
        }

        protected override AbstractQueue<string> CreateQueue(IEnumerable<string> enumerable)
        {
            return new ArrayQueue<string>(enumerable);
        }
    }
    
    public class ArrayQueueTest_Int : AbstractQueueTest<int>
    {
        protected override AbstractCollection<int> CreateCollection(int size)
        {
            return new ArrayQueue<int>(size);
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> enumerable)
        {
            return new ArrayQueue<int>(size, enumerable);
        }

        protected override AbstractQueue<int> CreateQueue()
        {
            return new ArrayQueue<int>();
        }

        protected override AbstractQueue<int> CreateQueue(IEnumerable<int> enumerable)
        {
            return new ArrayQueue<int>(enumerable);
        }
    }
}