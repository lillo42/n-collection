using System.Collections;

namespace NCollection.Test.Queue
{
    public class LinkedQueueTest : IQueueTest
    {
        protected override IQueue Create() 
            => new LinkedQueue();

        protected override IQueue Create(IEnumerable values) 
            => new LinkedQueue(values);
    }
}