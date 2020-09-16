using System.Collections.Generic;

namespace NCollection.Test.Queues
{
    public class LinkedQueueTest_String : AbstractQueueTest<string>
    {
        protected override bool ContainsInitialCapacity => false;

        protected override AbstractCollection<string> CreateCollection(int size)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractCollection<string> CreateCollection(int size, IEnumerable<string> source)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractQueue<string> CreateQueue()
        {
            return new LinkedQueue<string>();
        }

        protected override AbstractQueue<string> CreateQueue(IEnumerable<string> enumerable)
        {
            return new LinkedQueue<string>(enumerable);
        }
    }

    public class LinkedQueueTest_Int : AbstractQueueTest<int>
    {
        protected override bool ContainsInitialCapacity => false;
        
        protected override AbstractCollection<int> CreateCollection(int size)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> source)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractQueue<int> CreateQueue()
        {
            return new LinkedQueue<int>();
        }

        protected override AbstractQueue<int> CreateQueue(IEnumerable<int> enumerable)
        {
            return new LinkedQueue<int>(enumerable);
        }
    }
}