using System.Collections.Generic;
using NCollection.Generics;

namespace NCollection.Test.Generic.Queue
{
    public class LinkedQueueTest : IQueueTest<string>
    {
        protected override IQueue<string> Create() 
            => new LinkedQueue<string>();

        protected override IQueue<string> Create(IEnumerable<string> values) 
            => new LinkedQueue<string>(values);
    }
}