using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using NCollection.Generics;
using Xunit;

namespace NCollection.Test.Generic.Queue
{
    public abstract class IQueueTest<T>
    {
        protected Fixture Fixture { get; }

        protected IQueueTest()
        {
            Fixture = new Fixture();
        }

        protected abstract IQueue<T> Create();
        
        protected abstract IQueue<T> Create(IEnumerable<T> values);

        
        [Fact]
        public void CreateWithIEnumerable()
        {
            var values = Fixture.Create<T[]>();
            
            var queue = Create(values);
            queue.Count.Should().Be(values.Length);

            var i = 0;

            while (queue.TryDequeue(out var value))
            {
                value.Should().Be(values[i++]);
            }
        }
        
        [Fact]
        public void CreateWithIEnumerable_Throw() 
            => Assert.Throws<ArgumentNullException>(() => Create(null));


        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void Enqueue(int size)
        {
            var queue = Create();

            for (var i = 0; i < size; i++)
            {
                queue.Enqueue(Fixture.Create<T>());
            }
           
            queue.Count.Should().Be(size);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void Clear(int size)
        {
            var queue = Create();

            for (var i = 0; i < size; i++)
            {
                queue.Enqueue(Fixture.Create<T>());
            }
           
            queue.Count.Should().Be(size);
            queue.Clear();
            queue.Count.Should().Be(0);
        }
        
        [Fact]
        public void Peek()
        {
            var queue = Create();
            var data = Fixture.Create<T>();
            queue.Enqueue(data);
            
            var peek = queue.Peek();
            peek.Should().Be(data);
            queue.Count.Should().Be(1);
        }
        
        [Fact]
        public void Peek_Should_Throw_When_IsEmpty()
        {
            var queue = Create();

            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }

        [Fact]
        public void TryPeek_Should_BeTrue_When_HaveData()
        {
            var queue = Create();
            var data = Fixture.Create<T>();
            queue.Enqueue(data);
            
            var isPeeked = queue.TryPeek(out var peek);
            isPeeked.Should().BeTrue();
            peek.Should().Be(data);
            queue.Count.Should().Be(1);
        }
        
        [Fact]
        public void TryPeek_Should_BeFalse_When_HaveAny()
        {
            var queue = Create();
            
            var isPeeked = queue.TryPeek(out var peek);
            isPeeked.Should().BeFalse();
            peek.Should().BeNull();
            queue.Count.Should().Be(0);
        }
        
        [Fact]
        public void Dequeue()
        {
            var queue = Create();
            var data = Fixture.Create<T>();
            queue.Enqueue(data);
            
            var peek = queue.Dequeue();
            peek.Should().Be(data);
            queue.Count.Should().Be(0);
        }
        
        [Fact]
        public void Dequeue_Should_Throw_When_queueIsEmpty()
        {
            var queue = Create();
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        }
        
        [Fact]
        public void TryDequeue_Should_BeTrue_When_HaveData()
        {
            var queue = Create();
            var data = Fixture.Create<T>();
            queue.Enqueue(data);
            
            var isPeeked = queue.TryDequeue(out var peek);
            isPeeked.Should().BeTrue();
            peek.Should().Be(data);
            queue.Count.Should().Be(0);
        }
        
        [Fact]
        public void TryPop_Should_BeFalse_When_HaveAny()
        {
            var queue = Create();
            
            var isPeeked = queue.TryPeek(out var peek);
            isPeeked.Should().BeFalse();
            peek.Should().BeNull();
            queue.Count.Should().Be(0);
        }

        [Fact]
        public void Clone()
        {
            var array = Fixture.Create<T[]>();
            var queue = Create();

            foreach (var item in array)
            {
                queue.Enqueue(item);
            }

            var clone = queue.Clone();
            clone.Should().NotBeNull();
            var cloneEnumerator = clone!.GetEnumerator();
            var enumerator = queue.GetEnumerator();

            foreach (var value in array)
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
            var array = Fixture.Create<T[]>();
            var queue = Create();

            foreach (var item in array)
            {
                queue.Enqueue(item);
            }

            var clone = ((ICloneable)queue).Clone();
            clone.Should().NotBeNull();
            (clone is IQueue).Should().BeTrue();
            var cloneEnumerator = ((IQueue)clone).GetEnumerator();
            var enumerator = queue.GetEnumerator();

            foreach (var value in array)
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
        public void Contains_Should_BeTrue_When_HasValue()
        {
            var queue = Create();

            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                queue.Enqueue(value);
            }

            foreach (var value in values)
            {
                queue.Contains(value).Should().BeTrue();
            }
        }

        [Fact] 
        public void Contains_Should_BeFalse_When_DoesNotHaveValue()
        {
            var queue = Create();

            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                queue.Enqueue(value);
            }
            
            queue.Contains(Fixture.Create<T>()).Should().BeFalse();
        }
        
        [Fact] 
        public void CopyTo()
        {
            var queue = Create();

            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                queue.Enqueue(value);
            }
            
            var objArray = new T[values.Length];
            queue.CopyTo(objArray, 0);

            var enumerable = queue.GetEnumerator();

            foreach (var value in objArray)
            {
                enumerable.MoveNext().Should().BeTrue();
                value.Should().Be(enumerable.Current);
            }
            
            var strArray = new T[values.Length];
            queue.CopyTo(strArray, 0);
            
            enumerable.Reset();
            
            foreach (var value in strArray)
            {
                enumerable.MoveNext().Should().BeTrue();
                value.Should().Be(enumerable.Current);
            }
        }

        [Fact]
        public void Copy_Should_Throw()
        {
            var queue = Create();
            
            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                queue.Enqueue(value);
            }
            
            Assert.Throws<ArgumentNullException>(() => queue.CopyTo(null!, values.Length));
            Assert.Throws<ArgumentOutOfRangeException>(() => queue.CopyTo(new T[10], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => queue.CopyTo(new T[10], values.Length + 1));
            Assert.Throws<ArgumentException>(() => queue.CopyTo(new T[1], values.Length));
        }
        
        [Fact]
        public void CopyArray_Should_Throw()
        {
            var queue = Create();
            
            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                queue.Enqueue(value);
            }
            
            Assert.Throws<ArgumentNullException>(() => ((ICollection)queue).CopyTo(null!, values.Length));
            Assert.Throws<ArgumentException>(() => ((ICollection)queue).CopyTo(new object[10, 10], values.Length));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((ICollection)queue).CopyTo(new T[10], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ((ICollection)queue).CopyTo(new T[10], values.Length + 1));
            Assert.Throws<ArgumentException>(() => ((ICollection)queue).CopyTo(new T[1], values.Length));
        }
    }
}