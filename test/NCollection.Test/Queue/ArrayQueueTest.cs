using System.Collections;

namespace NCollection.Test.Queue
{
    public class ArrayQueueTest : IQueueTest
    {
        protected override IQueue Create() 
            => new ArrayQueue();

        protected override IQueue Create(IEnumerable values) 
            => new ArrayQueue(values);
    }
}