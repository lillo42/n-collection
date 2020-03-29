using System;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Stack
{
    public abstract class IStackTest
    {
        private Fixture Fixture { get; }

        protected IStackTest()
        {
            Fixture = new Fixture();
        }

        protected abstract IStack Create();

        [Fact]
        public void Push()
        {
            var stack = Create();
            stack.Push(Fixture.Create<object>());
            stack.Count.Should().Be(1);
            
            stack.Push(Fixture.Create<object>());
            stack.Count.Should().Be(2);
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

            var clone = stack.Clone();
            clone.Should().NotBeNull();
            var cloneEnumerator = clone.GetEnumerator();
            var enumerator = stack.GetEnumerator();

            for (var i = array.Length - 1; i >= 0; i--)
            {
                cloneEnumerator.MoveNext().Should().BeTrue();
                enumerator.MoveNext().Should().BeTrue();

                cloneEnumerator.Current.Should().Be(array[i]);
                enumerator.Current.Should().Be(array[i]);
            }
        }
    }
}