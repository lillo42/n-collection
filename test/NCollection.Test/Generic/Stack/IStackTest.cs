using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using NCollection.Generics;
using Xunit;

namespace NCollection.Test.Generic.Stack
{
    public abstract class IStackTest<T>
    {
        protected Fixture Fixture { get; }

        protected IStackTest()
        {
            Fixture = new Fixture();
        }

        protected abstract IStack<T> Create();
        
        protected abstract IStack<T> Create(IEnumerable<T> values);

        [Fact]
        public void CreateWithIEnumerable()
        {
            var values = Fixture.Create<T[]>();
            
            var stack = Create(values);
            stack.Count.Should().Be(values.Length);

            var i = values.Length - 1;

            while (stack.TryPop(out var value))
            {
                value.Should().Be(values[i--]);
            }
        }
        
        [Fact]
        public void CreateWithIEnumerable_Throw() 
            => Assert.Throws<ArgumentNullException>(() => Create(null));

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void Push(int size)
        {
            var stack = Create();

            for (var i = 0; i < size; i++)
            {
                stack.Push(Fixture.Create<T>());
            }
           
            stack.Count.Should().Be(size);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void Clear(int size)
        {
            var stack = Create();

            for (var i = 0; i < size; i++)
            {
                stack.Push(Fixture.Create<T>());
            }
           
            stack.Count.Should().Be(size);
            stack.Clear();
            stack.Count.Should().Be(0);
        }

        [Fact]
        public void PushCollection()
        {
            var values = Fixture.Create<T[]>();
            var stack = Create();
            stack.Push(values);
            
            stack.Count.Should().Be(values.Length);
            var i = values.Length - 1;

            while (stack.TryPop(out var value))
            {
                value.Should().Be(values[i--]);
            }
        }
        
        [Fact]
        public void Peek()
        {
            var stack = Create();
            var data = Fixture.Create<T>();
            stack.Push(data);
            
            var peek = stack.Peek();
            peek.Should().Be(data);
            stack.Count.Should().Be(1);
        }
        
        [Fact]
        public void Peek_Should_Throw_When_IsEmpty()
        {
            var stack = Create();

            Assert.Throws<InvalidOperationException>(() => stack.Peek());
        }

        [Fact]
        public void TryPeek_Should_BeTrue_When_HaveData()
        {
            var stack = Create();
            var data = Fixture.Create<T>();
            stack.Push(data);
            
            var isPeeked = stack.TryPeek(out var peek);
            isPeeked.Should().BeTrue();
            peek.Should().Be(data);
            stack.Count.Should().Be(1);
        }
        
        [Fact]
        public void TryPeek_Should_BeFalse_When_HaveAny()
        {
            var stack = Create();
            
            var isPeeked = stack.TryPeek(out var peek);
            isPeeked.Should().BeFalse();
            peek.Should().BeNull();
            stack.Count.Should().Be(0);
        }
        
        [Fact]
        public void Pop()
        {
            var stack = Create();
            var data = Fixture.Create<T>();
            stack.Push(data);
            
            var peek = stack.Pop();
            peek.Should().Be(data);
            stack.Count.Should().Be(0);
        }
        
        [Fact]
        public void Pop_Should_Throw_When_StackIsEmpty()
        {
            var stack = Create();
            Assert.Throws<InvalidOperationException>(() => stack.Pop());
        }
        
        [Fact]
        public void TryPop_Should_BeTrue_When_HaveData()
        {
            var stack = Create();
            var data = Fixture.Create<T>();
            stack.Push(data);
            
            var isPeeked = stack.TryPop(out var peek);
            isPeeked.Should().BeTrue();
            peek.Should().Be(data);
            stack.Count.Should().Be(0);
        }
        
        [Fact]
        public void TryPop_Should_BeFalse_When_HaveAny()
        {
            var stack = Create();
            
            var isPeeked = stack.TryPeek(out var peek);
            isPeeked.Should().BeFalse();
            peek.Should().BeNull();
            stack.Count.Should().Be(0);
        }

        [Fact]
        public void Clone()
        {
            var array = Fixture.Create<T[]>();
            var stack = Create();

            foreach (var item in array)
            {
                stack.Push(item);
            }

            var clone = stack.Clone();
            clone.Should().NotBeNull();
            var cloneEnumerator = clone!.GetEnumerator();
            var enumerator = stack.GetEnumerator();

            for (var i = array.Length - 1; i >= 0; i--)
            {
                cloneEnumerator.MoveNext().Should().BeTrue();
                enumerator.MoveNext().Should().BeTrue();

                cloneEnumerator.Current.Should().Be(array[i]);
                enumerator.Current.Should().Be(array[i]);
            }
            
            cloneEnumerator.MoveNext().Should().BeFalse();
            enumerator.MoveNext().Should().BeFalse();
        }
        
        
        [Fact]
        public void  IClonable_Clone()
        {
            var array = Fixture.Create<T[]>();
            var stack = Create();

            foreach (var item in array)
            {
                stack.Push(item);
            }

            var clone = ((ICloneable)stack).Clone();
            clone.Should().NotBeNull();
            (clone is IStack).Should().BeTrue();
            var cloneEnumerator = ((IStack)clone).GetEnumerator();
            var enumerator = stack.GetEnumerator();

            for (var i = array.Length - 1; i >= 0; i--)
            {
                cloneEnumerator.MoveNext().Should().BeTrue();
                enumerator.MoveNext().Should().BeTrue();

                cloneEnumerator.Current.Should().Be(array[i]);
                enumerator.Current.Should().Be(array[i]);
            }
            
            cloneEnumerator.MoveNext().Should().BeFalse();
            enumerator.MoveNext().Should().BeFalse();
        }
        
        [Fact]
        public void Contains_Should_BeTrue_When_HasValue()
        {
            var stack = Create();

            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }

            foreach (var value in values)
            {
                stack.Contains(value).Should().BeTrue();
            }

        }

        [Fact] 
        public void Contains_Should_BeFalse_When_DoesNotHaveValue()
        {
            var stack = Create();

            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }
            
            stack.Contains(Fixture.Create<T>()).Should().BeFalse();
        }
        
        [Fact] 
        public void CopyTo()
        {
            var stack = Create();

            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }
            
            var objArray = new T[values.Length];
            stack.CopyTo(objArray, 0);

            var enumerable = stack.GetEnumerator();

            foreach (var value in objArray)
            {
                enumerable.MoveNext().Should().BeTrue();
                value.Should().Be(enumerable.Current);
            }
            
            var strArray = new T[values.Length];
            stack.CopyTo(strArray, 0);
            
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
            var stack = Create();
            
            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }
            
            Assert.Throws<ArgumentNullException>(() => stack.CopyTo(null!, values.Length));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.CopyTo(new T[10], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.CopyTo(new T[10], values.Length + 1));
            Assert.Throws<ArgumentException>(() => stack.CopyTo(new T[1], values.Length));
        }
    }
}