using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Stack
{
    public abstract class AbstractStackTest<T> : AbstractionCollectionTest<T>
    {
        protected abstract AbstractStack<T> CreateStack();

        protected virtual AbstractStack<T> CreateStack(IEnumerable<T> enumerable)
        {
            var stack = CreateStack();
            foreach (var item in enumerable)
            {
                stack.Push(item);
            }

            return stack;
        }

        protected override AbstractCollection<T> CreateCollection()
        {
            return CreateStack();
        }

        protected override AbstractCollection<T> CreateCollection(IEnumerable<T> array)
        {
            return CreateStack(array);
        }

        #region Push

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractStackTest_Push_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateStack();

            foreach (var item in array)
            {
                collection.Push(item);
            }

            collection.Should().HaveCount(size);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractStackTest_Push_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateStack();

            foreach (var item in array)
            {
                collection.Push(item);
            }
            
            collection.Should().HaveCount(size);
            
            collection.Clear();
            
            foreach (var item in array)
            {
                collection.Push(item);
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
        public void AbstractStackTest_TryPush_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateStack();

            foreach (var item in array)
            {
                collection.TryPush(item).Should().BeTrue();
            }

            collection.Should().HaveCount(size);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractStackTest_TryPush_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateStack();

            foreach (var item in array)
            {
                collection.TryPush(item).Should().BeTrue();
            }
            
            collection.Should().HaveCount(size);
            
            collection.Clear();
            
            foreach (var item in array)
            {
                collection.Push(item);
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
        public void AbstractStackTest_Peek_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var stack = CreateStack();

            foreach (var item in array)
            {
                stack.Push(item);
            }

            stack.Should().HaveCount(size);
            stack.Peek().Should().Be(array[size - 1]);
            stack.Should().HaveCount(size);
        }
        
        [Fact]
        public void AbstractStackTest_Peek_Throw()
        {
            var stack = CreateStack();
            Assert.Throws<InvalidOperationException>(() => stack.Peek());
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractStackTest_Peek_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var stack = CreateStack();

            foreach (var item in array)
            {
                stack.Push(item);
            }
            
            stack.Should().HaveCount(size);
            
            stack.Clear();
            
            foreach (var item in array)
            {
                stack.Push(item);
            }
            
            stack.Should().HaveCount(size);
            stack.Peek().Should().Be(array[size - 1]);
        }
        
        
        [Fact]
        public void AbstractStackTest_Peek_AfterClear_Throw()
        {
            var array = CreateAValidArray(10);
            var stack = CreateStack(array);
            stack.Clear();
            Assert.Throws<InvalidOperationException>(() => stack.Peek());
        }

        #endregion
        
        #region TryPeek

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractStackTest_TryPeek_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var stack = CreateStack();

            foreach (var item in array)
            {
                stack.TryPush(item).Should().BeTrue();
            }

            stack.Should().HaveCount(size);
            stack.TryPeek(out var peek).Should().BeTrue();
            peek.Should().Be(array[size - 1]);
        }
        
        [Fact]
        public void AbstractStackTest_TryPeek_Invalid()
        {
            var stack = CreateStack();
            stack.TryPeek(out var peek).Should().BeFalse();
            peek.Should().Be(default(T));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractStackTest_TryPeek_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var stack = CreateStack();

            foreach (var item in array)
            {
                stack.TryPush(item).Should().BeTrue();
            }
            
            stack.Should().HaveCount(size);
            
            stack.Clear();
            
            foreach (var item in array)
            {
                stack.Push(item);
            }
            
            stack.Should().HaveCount(size);
            stack.TryPeek(out var peek).Should().BeTrue();
            peek.Should().Be(array[size - 1]);
            stack.Should().HaveCount(size);
        }
        
        [Fact]
        public void AbstractStackTest_TryPeek_AfterClear_Invalid()
        {
            var array = CreateAValidArray(10);
            var stack = CreateStack(array);
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
        public void AbstractStackTest_Pop_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var stack = CreateStack();

            foreach (var item in array)
            {
                stack.Push(item);
            }

            stack.Should().HaveCount(size);
            stack.Pop().Should().Be(array[size - 1]);
            stack.Should().HaveCount(size - 1);
        }
        
        [Fact]
        public void AbstractStackTest_Pop_Throw()
        {
            var stack = CreateStack();
            Assert.Throws<InvalidOperationException>(() => stack.Pop());
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractStackTest_Pop_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var stack = CreateStack(array);
            
            stack.Should().HaveCount(size);
            
            stack.Clear();

            stack.AddAll(array);
            
            stack.Should().HaveCount(size);
            stack.Pop().Should().Be(array[size - 1]);
            stack.Should().HaveCount(size - 1);
        }
        
        
        [Fact]
        public void AbstractStackTest_Pop_AfterClear_Throw()
        {
            var array = CreateAValidArray(10);
            var stack = CreateStack(array);
            stack.Clear();
            Assert.Throws<InvalidOperationException>(() => stack.Pop());
        }

        #endregion
        
        #region TryPeek

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractStackTest_TryPop_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var stack = CreateStack(array);

            stack.Should().HaveCount(size);
            stack.TryPop(out var peek).Should().BeTrue();
            peek.Should().Be(array[size - 1]);
            stack.Should().HaveCount(size - 1);
        }
        
        [Fact]
        public void AbstractStackTest_TryPop_Invalid()
        {
            var stack = CreateStack();
            stack.TryPop(out var peek).Should().BeFalse();
            peek.Should().Be(default(T));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractStackTest_TryPop_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var stack = CreateStack(array);
            
            
            stack.Should().HaveCount(size);
            
            stack.Clear();

            stack.AddAll(array);
            
            stack.Should().HaveCount(size);
            stack.TryPop(out var peek).Should().BeTrue();
            peek.Should().Be(array[size - 1]);
            stack.Should().HaveCount(size -1);
        }
        
        [Fact]
        public void AbstractStackTest_TryPop_AfterClear_Invalid()
        {
            var array = CreateAValidArray(10);
            var stack = CreateStack(array);
            stack.Clear();
            stack.TryPop(out var peek).Should().BeFalse();
            peek.Should().Be(default(T));
        }

        #endregion

        #region ToArray
        
        public override void AbstractionCollectionTest_ToArray(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection(array);

            collection.ToArray().Should().BeEquivalentTo(array.Reverse());
        }

        #endregion
    }
}