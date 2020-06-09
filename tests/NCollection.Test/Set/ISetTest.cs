using System;
using System.Collections;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Set
{
    public abstract class ISetTest
    {
        protected Fixture Fixture { get; }

        protected ISetTest()
        {
            Fixture = new Fixture();
        }

        protected abstract ISet Create();
        
        protected abstract ISet Create(IEnumerable values);
        
        [Fact]
        public void CreateWithIEnumerable()
        {
            var values = Fixture.Create<string[]>();
            
            var list = Create(values);
            list.Count.Should().Be(values.Length);
            
            for (var i = 0; i < values.Length; i++)
            {
                list.Contains(values[i]).Should().BeTrue();
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
                stack.Add(Fixture.Create<object>()).Should().BeTrue();
            }
           
            stack.Count.Should().Be(size);
        }

        [Fact]
        public void Remove()
        {
            var items = Fixture.Create<string[]>();
            var list = Create(items);
            list.Remove(items[1]).Should().BeTrue();
            list.Should().HaveCount(items.Length -1);
            
            list.Add(null);
            list.Remove(null).Should().BeTrue();
            list.Should().HaveCount(items.Length -1);
        }
        
        [Fact]
        public void Remove_Should_ReturnFalse_When_ItemNotExist()
        {
            var items = Fixture.Create<object[]>();
            var list = Create(items);
            list.Remove(Fixture.Create<object>()).Should().BeFalse();
            list.Should().HaveCount(items.Length);
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
            var array = Fixture.Create<object[]>();
            var collection = Create();

            foreach (var item in array)
            {
                collection.Add(item).Should().BeTrue();
            }

            var clone = (ISet)collection.Clone();
            clone.Should().NotBeNull();
            clone.Should().HaveCount(collection.Count);
            
            var cloneEnumerator = clone!.GetEnumerator();
            var enumerator = collection.GetEnumerator();

            for (var i = 0; i < array.Length; i++)
            {
                cloneEnumerator.MoveNext().Should().BeTrue();
                enumerator.MoveNext().Should().BeTrue();

                array.Should().Contain(cloneEnumerator.Current);
                array.Should().Contain(enumerator.Current);
            }
            
            cloneEnumerator.MoveNext().Should().BeFalse();
            enumerator.MoveNext().Should().BeFalse();
        }
        
        
        [Fact]
        public void  IClonable_Clone()
        {
            var array = Fixture.Create<object[]>();
            var collection = Create();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            var clone = collection.Clone();
            clone.Should().NotBeNull();
            (clone is ISet).Should().BeTrue();
            var cloneEnumerator = ((ISet)clone).GetEnumerator();
            var enumerator = collection.GetEnumerator();

            for (var i = 0; i < array.Length; i++)
            {
                cloneEnumerator.MoveNext().Should().BeTrue();
                enumerator.MoveNext().Should().BeTrue();

                array.Should().Contain(cloneEnumerator.Current);
                array.Should().Contain(enumerator.Current);
            }
            
            cloneEnumerator.MoveNext().Should().BeFalse();
            enumerator.MoveNext().Should().BeFalse();
        }
        
        [Fact]
        public void Contains_Should_BeTrue_When_HasValue()
        {
            var collection = Create();

            var values = Fixture.Create<object[]>();

            foreach (var value in values)
            {
                collection.Add(value);
            }
            
            collection.Add(null);

            foreach (var value in values)
            {
                collection.Contains(value).Should().BeTrue();
            }

            collection.Contains(null).Should().BeTrue();
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
            var collection = Create();

            var values = Fixture.Create<string[]>();

            foreach (var value in values)
            {
                collection.Add(value);
            }
            
            var objArray = new object[values.Length];
            collection.CopyTo(objArray, 0);
            
            foreach (var value in collection)
            {
                objArray.Should().Contain(value);
            }
            
            var strArray = new string[values.Length];
            collection.CopyTo(strArray, 0);
            
            foreach (var value in collection)
            {
                strArray.Should().Contain((string)value);
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