using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Queue
{
    public class ArrayQueueTest : IQueueTest
    {
        protected override IQueue Create() 
            => new ArrayQueue();

        protected override IQueue Create(IEnumerable values) 
            => new ArrayQueue(values);

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