using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NCollection.Generics;
using Xunit;

namespace NCollection.Test.Generic.Queue
{
    public class ArrayQueueTest : IQueueTest<string>
    {
        protected override IQueue<string> Create() 
            => new ArrayQueue<string>();

        protected override IQueue<string> Create(IEnumerable<string> values) 
            => new ArrayQueue<string>(values);

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        public void EnqueueDequeueEnqueue(int size)
        {
            var queue = Create();
            
            var values = new List<string>();
            for (var i = 0; i < size; i++)
            {
                values.Add(Fixture.Create<string>());
                queue.Enqueue(values[i]);
            }
            
            for (var i = 0; i < size / 2; i++)
            {
                values.RemoveAt(0);
                queue.Dequeue();
            }
            
            for (var i = 0; i < size; i++)
            {
                values.Add(Fixture.Create<string>());
                queue.Enqueue(values.Last());
            }

            int count = 0;
            foreach (var value in queue)
            {
                value.Should().Be(values[count++]);
            }
        }
    }
}