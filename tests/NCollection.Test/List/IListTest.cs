using System;
using System.Collections;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.List
{
    public abstract class IListTest
    {
        protected Fixture Fixture { get; }

        protected IListTest()
        {
            Fixture = new Fixture();
        }

        protected abstract IList Create();
        
        protected abstract IList Create(IEnumerable values);

        
        [Fact]
        public void CreateWithIEnumerable()
        {
            var values = Fixture.Create<string[]>();
            
            var list = Create(values);
            list.Count.Should().Be(values.Length);
            
            for (var i = 0; i < values.Length; i++)
            {
                list[i].Should().Be(values[i]);
            }
        }
        
        [Fact]
        public void CreateWithIEnumerable_Throw() 
            => Assert.Throws<ArgumentNullException>(() => Create(null));


        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void Add(int size)
        {
            var stack = Create();

            for (var i = 0; i < size; i++)
            {
                stack.Add(Fixture.Create<object>());
            }
           
            stack.Count.Should().Be(size);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void Clear(int size)
        {
            var list = Create();

            for (var i = 0; i < size; i++)
            {
                list.Add(Fixture.Create<object>());
            }
           
            list.Count.Should().Be(size);
            list.Clear();
            list.Count.Should().Be(0);
        }

        [Fact]
        public void Clone()
        {
            var array = Fixture.Create<string[]>();
            var list = Create();

            foreach (var item in array)
            {
                list.Add(item);
            }

            var clone = (IList)list.Clone();
            clone.Should().NotBeNull();
            var cloneEnumerator = clone!.GetEnumerator();
            var enumerator = list.GetEnumerator();

            for (var i = 0; i < array.Length; i++)
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
                stack.Add(item);
            }

            var clone = stack.Clone();
            clone.Should().NotBeNull();
            (clone is IList).Should().BeTrue();
            var cloneEnumerator = ((IList)clone).GetEnumerator();
            var enumerator = stack.GetEnumerator();

            for (var i = 0; i < array.Length; i++)
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
                stack.Add(value);
            }
            
            stack.Add(null);

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
                stack.Add(value);
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
                stack.Add(value);
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
                stack.Add(value);
            }
            
            Assert.Throws<ArgumentNullException>(() => stack.CopyTo(null!, values.Length));
            Assert.Throws<ArgumentException>(() => stack.CopyTo(new object[10, 10], values.Length));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.CopyTo(new object[10], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.CopyTo(new object[10], values.Length + 1));
            Assert.Throws<ArgumentException>(() => stack.CopyTo(new object[1], values.Length));
        }
    }
}