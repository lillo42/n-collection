using System;
using System.Collections;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Queue
{
    public class PriorityQueueTest
    {
        private readonly Fixture _fixture;

        public PriorityQueueTest()
        {
            _fixture = new Fixture();
        }


        #region Contructor

        [Fact]
        public void ContructorWithInitialCapacity_Should_Throw_When_InitialCapacityIsLessTanZero()
            => Assert.Throws<ArgumentOutOfRangeException>(() => new PriorityQueue(-1));
        
        [Fact]
        public void ContructorComparer_Should_Throw_When_ComparerIsNull()
            => Assert.Throws<ArgumentNullException>(() => new PriorityQueue((IComparer)null));
        
        [Fact]
        public void ContructorWithInitialCapacityAndComparer_Should_Throw_When_initialCapacityIsLessTanZero()
            => Assert.Throws<ArgumentOutOfRangeException>(() => new PriorityQueue(-1, Comparer.Default));
        
        [Fact]
        public void ContructorWithInitialCapacityAndComparer_Should_Throw_When_ComparerIsNull()
            => Assert.Throws<ArgumentNullException>(() => new PriorityQueue(0, null!));
        
        [Fact]
        public void ContructorEnumerable_Should_Throw_When_EnumerableIsNull()
            => Assert.Throws<ArgumentNullException>(() => new PriorityQueue((IEnumerable)null));
        
        [Fact]
        public void ContructorEnumerableAndComparer_Should_Throw_When_EnumerableIsNull()
            => Assert.Throws<ArgumentNullException>(() => new PriorityQueue((IEnumerable)null, Comparer.Default));
        
        [Fact]
        public void ContructorEnumerableAndComparer_Should_Throw_When_ComparerIsNull()
            => Assert.Throws<ArgumentNullException>(() => new PriorityQueue(new ArrayList(), null));

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void Contructor(int size)
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            
            for (var i = 0; i < size; i++)
            {
                var value = _fixture.Create<int>();
                queue.Enqueue(value);
            }
            
            var newQueue = new PriorityQueue(queue);
            newQueue.Count.Should().Be(queue.Count);
            queue.Count.Should().Be(size);

            while (queue.TryDequeue(out var item1) && newQueue.TryDequeue(out var item2))
            {
                item1.Should().Be(item2);
            }

            queue.Should().BeEmpty();
            newQueue.Should().BeEmpty();
        }
        
        #endregion

        #region Enqueue
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void Enqueue(int size)
        {
            var queue = new PriorityQueue(Comparer<int>.Default);

            var values = new List<int>(size);
            for (var i = 0; i < size; i++)
            {
                var value = _fixture.Create<int>();
                queue.Enqueue(value);
                values.Add(value);
            }
            
            values.Sort(Comparer<int>.Default);
            queue.Count.Should().Be(size);
            
            foreach (var expected in values)
            {
                queue.TryDequeue(out var value).Should().BeTrue();
                value.Should().Be(expected);
            }
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void IQueue_Enqueue(int size)
        {
            IQueue queue = new PriorityQueue(Comparer<int>.Default);

            var values = new List<int>(size);
            for (var i = 0; i < size; i++)
            {
                var value = _fixture.Create<int>();
                queue.Enqueue(value);
                values.Add(value);
            }
            
            values.Sort(Comparer<int>.Default);
            queue.Count.Should().Be(size);
            
            foreach (var expected in values)
            {
                queue.TryDequeue(out var value).Should().BeTrue();
                value.Should().Be(expected);
            }
        }
        
        [Fact]
        public void IQueue_Enqueue_Should_Throw_When_ItemIsNull()
        {
            IQueue queue = new PriorityQueue();
            Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null));
        }
        #endregion

        #region Peek

        [Fact]
        public void Peek()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            var data = _fixture.Create<int>();
            queue.Enqueue(data);
            
            var peek = queue.Peek();
            peek.Should().Be(data);
            queue.Count.Should().Be(1);
        }
        
        [Fact]
        public void Peek_Should_Throw_When_IsEmpty()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }
        
        [Fact]
        public void TryPeek_Should_BeTrue_When_HaveData()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            var data = _fixture.Create<int>();
            queue.Enqueue(data);
            
            var isPeeked = queue.TryPeek(out var peek);
            isPeeked.Should().BeTrue();
            peek.Should().Be(data);
            queue.Count.Should().Be(1);
        }
        
        [Fact]
        public void TryPeek_Should_BeFalse_When_HaveAny()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            
            var isPeeked = queue.TryPeek(out var peek);
            isPeeked.Should().BeFalse();
            peek.Should().BeNull();
            queue.Count.Should().Be(0);
        }

        #endregion

        #region Dequeue

        [Fact]
        public void Dequeue()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            var data = _fixture.Create<int>();
            queue.Enqueue(data);
            
            var peek = queue.Dequeue();
            peek.Should().Be(data);
            queue.Count.Should().Be(0);
        }
        
        [Fact]
        public void Dequeue_Should_Throw_When_queueIsEmpty()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        }
        
        [Fact]
        public void TryDequeue_Should_BeTrue_When_HaveData()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            var data = _fixture.Create<int>();
            queue.Enqueue(data);
            
            var isPeeked = queue.TryDequeue(out var peek);
            isPeeked.Should().BeTrue();
            peek.Should().Be(data);
            queue.Count.Should().Be(0);
        }
        
        [Fact]
        public void TryPop_Should_BeFalse_When_HaveAny()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            
            var isPeeked = queue.TryPeek(out var peek);
            isPeeked.Should().BeFalse();
            peek.Should().BeNull();
            queue.Count.Should().Be(0);
        }

        #endregion

        #region Contains

        [Fact]
        public void Contains_Should_BeTrue_When_HasValue()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            var data = _fixture.Create<int[]>();

            foreach (var value in data)
            {
                queue.Enqueue(value);
            }

            foreach (var value in data)
            {
                queue.Contains(value).Should().BeTrue();
            }
        }

        [Fact] 
        public void Contains_Should_BeFalse_When_DoesNotHaveValue()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            var data = _fixture.Create<int[]>();

            foreach (var value in data)
            {
                queue.Enqueue(value);
            }
            
            queue.Contains(_fixture.Create<int>()).Should().BeFalse();
            queue.Contains(null).Should().BeFalse();
        }

        #endregion

        #region CopyTo

        [Fact] 
        public void CopyTo()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            var data = _fixture.Create<int[]>();

            foreach (var value in data)
            {
                queue.Enqueue(value);
            }
            
            var objArray = new object[data.Length];
            queue.CopyTo(objArray, 0);

            var enumerable = queue.GetEnumerator();

            foreach (var value in objArray)
            {
                enumerable.MoveNext().Should().BeTrue();
                value.Should().Be(enumerable.Current);
            }
            
            var strArray = new int[data.Length];
            queue.CopyTo(strArray, 0);
            
            enumerable.Reset();
            
            foreach (var value in strArray)
            {
                enumerable.MoveNext().Should().BeTrue();
                value.Should().Be((int)enumerable.Current);
            }
        }

        [Fact]
        public void Copy_Should_Throw()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            var data = _fixture.Create<int[]>();

            foreach (var value in data)
            {
                queue.Enqueue(value);
            }
            
            Assert.Throws<ArgumentNullException>(() => queue.CopyTo(null!, data.Length));
            Assert.Throws<ArgumentException>(() => queue.CopyTo(new object[10, 10], data.Length));
            Assert.Throws<ArgumentOutOfRangeException>(() => queue.CopyTo(new object[10], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => queue.CopyTo(new object[10], data.Length + 1));
            Assert.Throws<ArgumentException>(() => queue.CopyTo(new object[1], data.Length));
        }

        #endregion

        #region Clone

        [Fact]
        public void Clone()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            var data = _fixture.Create<List<int>>();

            foreach (var item in data)
            {
                queue.Enqueue(item);
            }

            var clone = (IQueue)queue.Clone();
            clone.Should().NotBeNull();
            var cloneEnumerator = clone!.GetEnumerator();
            var enumerator = queue.GetEnumerator();

            data.Sort();
            foreach (var value in data)
            {
                cloneEnumerator.MoveNext().Should().BeTrue();
                enumerator.MoveNext().Should().BeTrue();

                cloneEnumerator.Current.Should().Be(value);
                enumerator.Current.Should().Be(value);
            }
            
            cloneEnumerator.MoveNext().Should().BeFalse();
            enumerator.MoveNext().Should().BeFalse();
        }
        
        
        [Fact]
        public void  IClonable_Clone()
        {
            var queue = new PriorityQueue(Comparer<int>.Default);
            var data = _fixture.Create<List<int>>();

            foreach (var item in data)
            {
                queue.Enqueue(item);
            }

            var clone = ((ICloneable)queue).Clone();
            clone.Should().NotBeNull();
            (clone is IQueue).Should().BeTrue();
            var cloneEnumerator = ((IQueue)clone).GetEnumerator();
            var enumerator = queue.GetEnumerator();

            data.Sort();
            foreach (var value in data)
            {
                cloneEnumerator.MoveNext().Should().BeTrue();
                enumerator.MoveNext().Should().BeTrue();

                cloneEnumerator.Current.Should().Be(value);
                enumerator.Current.Should().Be(value);
            }
            
            cloneEnumerator.MoveNext().Should().BeFalse();
            enumerator.MoveNext().Should().BeFalse();
        }

        #endregion
    }
}