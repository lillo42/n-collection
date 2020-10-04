using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Queues
{
    public abstract class AbstractQueueTest<T> : AbstractionCollectionTest<T>
    {
        protected abstract AbstractQueue<T> CreateQueue();

        protected virtual AbstractQueue<T> CreateQueue(IEnumerable<T> enumerable)
        {
            var stack = CreateQueue();
            foreach (var item in enumerable)
            {
                stack.TryEnqueue(item);
            }

            return stack;
        }

        protected override AbstractCollection<T> CreateCollection()
        {
            return CreateQueue();
        }

        protected override AbstractCollection<T> CreateCollection(IEnumerable<T> array)
        {
            return CreateQueue(array);
        }
        
        #region Enqueue
        public void AbstractQueueTest_Enqueue_AfterClear(int size)
        {
            var array = CreatArray(size);
            var collection = CreateQueue();

            foreach (var item in array)
            {
                collection.Enqueue(item);
            }
            
            collection.Should().HaveCount(size);
            
            collection.Clear();
            
            foreach (var item in array)
            {
                collection.Enqueue(item);
            }
            
            collection.Should().HaveCount(size);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_Enqueue_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateQueue();

            foreach (var item in array)
            {
                collection.Enqueue(item);
            }

            collection.Should().HaveCount(size);
        }

        #endregion
        
        #region TryPush

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_TryEnqueue_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateQueue();

            foreach (var item in array)
            {
                collection.TryEnqueue(item).Should().BeTrue();
            }

            collection.Should().HaveCount(size);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_TryEnqueue_AfterClear(int size)
        {
            var array = CreatArray(size);
            var collection = CreateQueue();

            foreach (var item in array)
            {
                collection.TryEnqueue(item).Should().BeTrue();
            }
            
            collection.Should().HaveCount(size);
            
            collection.Clear();
            
            foreach (var item in array)
            {
                collection.Enqueue(item);
            }
            
            collection.Should().HaveCount(size);
        }

        #endregion
        
        #region Peek

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_Peek_Validity(int size)
        {
            var array = CreatArray(size);
            var queue = CreateQueue(array);

            queue.Should().HaveCount(size);
            queue.Peek().Should().Be(GetPeekValue(array));
            queue.Should().HaveCount(size);
        }
        
        [Fact]
        public void AbstractQueueTest_Peek_Throw()
        {
            var stack = CreateQueue();
            Assert.Throws<InvalidOperationException>(() => stack.Peek());
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_Peek_AfterClear(int size)
        {
            var array = CreatArray(size);
            var queue = CreateQueue(array);
            
            
            queue.Should().HaveCount(size);
            
            queue.Clear();
            
            foreach (var item in array)
            {
                queue.Enqueue(item);
            }
            
            queue.Should().HaveCount(size);
            queue.Peek().Should().Be(GetPeekValue(array));
        }
        
        
        [Fact]
        public void AbstractQueueTest_Peek_AfterClear_Throw()
        {
            var array = CreatArray(10);
            var queue = CreateQueue(array);
            queue.Clear();
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }

        #endregion
        
        #region TryPeek

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_TryPeek_Validity(int size)
        {
            var array = CreatArray(size);
            var stack = CreateQueue();

            foreach (var item in array)
            {
                stack.TryEnqueue(item).Should().BeTrue();
            }

            stack.Should().HaveCount(size);
            stack.TryPeek(out var peek).Should().BeTrue();
            peek.Should().Be(GetPeekValue(array));
        }

        protected virtual T GetPeekValue(T[] array)
        {
            return array[0];
        }
        
        [Fact]
        public void AbstractQueueTest_TryPeek_Invalid()
        {
            var queue = CreateQueue();
            queue.TryPeek(out var peek).Should().BeFalse();
            peek.Should().Be(default(T));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_TryPeek_AfterClear(int size)
        {
            var array = CreatArray(size);
            var queue = CreateQueue(array);
            
            
            queue.Should().HaveCount(size);
            
            queue.Clear();
            
            foreach (var item in array)
            {
                queue.TryEnqueue(item);
            }
            
            queue.Should().HaveCount(size);
            queue.TryPeek(out var peek).Should().BeTrue();
            peek.Should().Be(GetPeekValue(array));
            queue.Should().HaveCount(size);
        }
        
        [Fact]
        public void AbstractQueueTest_TryPeek_AfterClear_Invalid()
        {
            var array = CreatArray(10);
            var stack = CreateQueue(array);
            stack.Clear();
            stack.TryPeek(out var peek).Should().BeFalse();
            peek.Should().Be(default(T));
        }

        #endregion
        
        #region Pop

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_Dequeue_Validity(int size)
        {
            var array = CreatArray(size);
            var stack = CreateQueue(array);

            stack.Should().HaveCount(size);
            stack.Dequeue().Should().Be(GetPeekValue(array));
            stack.Should().HaveCount(size - 1);
            
        }
        
        [Fact]
        public void AbstractQueueTest_Dequeue_Throw()
        {
            var queue = CreateQueue();
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_Dequeue_AfterClear(int size)
        {
            var array = CreatArray(size);
            var stack = CreateQueue(array);
            
            stack.Should().HaveCount(size);
            
            stack.Clear();

            stack.AddAll(array);
            
            stack.Should().HaveCount(size);
            stack.Dequeue().Should().Be(GetPeekValue(array));
            stack.Should().HaveCount(size - 1);
        }
        
        
        [Fact]
        public void AbstractQueueTest_Dequeue_AfterClear_Throw()
        {
            var array = CreatArray(10);
            var stack = CreateQueue(array);
            stack.Clear();
            Assert.Throws<InvalidOperationException>(() => stack.Dequeue());
        }

        #endregion
        
        #region TryPeek

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_TryDequeue_Validity(int size)
        {
            var array = CreatArray(size);
            var stack = CreateQueue(array);

            stack.Should().HaveCount(size);
            stack.TryDequeue(out var peek).Should().BeTrue();
            peek.Should().Be(GetPeekValue(array));
            stack.Should().HaveCount(size - 1);
        }
        
        [Fact]
        public void AbstractQueueTest_TryDequeue_Invalid()
        {
            var stack = CreateQueue();
            stack.TryDequeue(out var peek).Should().BeFalse();
            peek.Should().Be(default(T));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractQueueTest_TryDequeue_AfterClear(int size)
        {
            var array = CreatArray(size);
            var stack = CreateQueue(array);
            
            
            stack.Should().HaveCount(size);
            
            stack.Clear();

            stack.AddAll(array);
            
            stack.Should().HaveCount(size);
            stack.TryDequeue(out var peek).Should().BeTrue();
            peek.Should().Be(GetPeekValue(array));
            stack.Should().HaveCount(size -1);
        }
        
        [Fact]
        public void AbstractQueueTest_TryDequeue_AfterClear_Invalid()
        {
            var array = CreatArray(10);
            var stack = CreateQueue(array);
            stack.Clear();
            stack.TryDequeue(out var peek).Should().BeFalse();
            peek.Should().Be(default(T));
        }

        #endregion
    }
}