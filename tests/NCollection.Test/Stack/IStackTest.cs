using System;
using System.Collections;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Stack
{
    public abstract class IStackTest
    {
        protected Fixture Fixture { get; }

        protected IStackTest()
        {
            var a = new System.Collections.Generic.HashSet<object>();
            Fixture = new Fixture();
        }

        protected abstract IStack Create();
        
        protected abstract IStack Create(IEnumerable values);

        
        [Fact]
        public void CreateWithIEnumerable()
        {
            var values = Fixture.Create<string[]>();
            
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
                stack.Push(Fixture.Create<object>());
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
                stack.Push(Fixture.Create<object>());
            }
           
            stack.Count.Should().Be(size);
            stack.Clear();
            stack.Count.Should().Be(0);
        }
        
        [Fact]
        public void Peek()
        {
            var stack = Create();
            var data = Fixture.Create<object>();
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
            var data = Fixture.Create<object>();
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
            var data = Fixture.Create<object>();
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
            var data = Fixture.Create<object>();
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
            var array = Fixture.Create<string[]>();
            var stack = Create();

            foreach (var item in array)
            {
                stack.Push(item);
            }

            var clone = (IStack)stack.Clone();
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
            var array = Fixture.Create<string[]>();
            var stack = Create();

            foreach (var item in array)
            {
                stack.Push(item);
            }

            var clone = stack.Clone();
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

            var values = Fixture.Create<object[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }
            
            stack.Push(null);

            foreach (var value in values)
            {
                stack.Contains(value).Should().BeTrue();
            }

            stack.Contains(null).Should().BeTrue();
        }

        [Fact] 
        public void Contains_Should_BeFalse_When_DoesNotHaveValue()
        {
            var stack = Create();

            var values = Fixture.Create<object[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }
            
            stack.Contains(Fixture.Create<object>()).Should().BeFalse();
            stack.Contains(null).Should().BeFalse();
        }
        
        [Fact] 
        public void CopyTo()
        {
            var stack = Create();

            var values = Fixture.Create<string[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }
            
            var objArray = new object[values.Length];
            stack.CopyTo(objArray, 0);

            var enumerable = stack.GetEnumerator();

            foreach (var value in objArray)
            {
                enumerable.MoveNext().Should().BeTrue();
                value.Should().Be(enumerable.Current);
            }
            
            var strArray = new string[values.Length];
            stack.CopyTo(strArray, 0);
            
            enumerable.Reset();
            
            foreach (var value in strArray)
            {
                enumerable.MoveNext().Should().BeTrue();
                value.Should().Be((string)enumerable.Current);
            }
        }

        [Fact]
        public void Copy_Should_Throw()
        {
            var stack = Create();
            
            var values = Fixture.Create<object[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }
            
            Assert.Throws<ArgumentNullException>(() => stack.CopyTo(null!, values.Length));
            Assert.Throws<ArgumentException>(() => stack.CopyTo(new object[10, 10], values.Length));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.CopyTo(new object[10], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.CopyTo(new object[10], values.Length + 1));
            Assert.Throws<ArgumentException>(() => stack.CopyTo(new object[1], values.Length));
        }
    }
}