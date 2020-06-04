using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Xunit;
namespace NCollection.Test.Generic.List
{
    public abstract class IListTest<T>
    {
        protected Fixture Fixture { get; }

        protected IListTest()
        {
            Fixture = new Fixture();
        }

        protected abstract NCollection.Generics.IList<T> Create();
        
        protected abstract NCollection.Generics.IList<T> Create(IEnumerable<T> values);
        
        [Fact]
        public void CreateWithIEnumerable()
        {
            var values = Fixture.Create<T[]>();
            
            var list = Create(values);
            list.Count.Should().Be(values.Length);
            
            for (var i = 0; i < values.Length; i++)
            {
                list[i].Should().Be(values[i]);
            }
        }
        
        [Fact]
        public void CreateWithIEnumerable_Throw() 
            => Assert.Throws<ArgumentNullException>(() => Create(default));


        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        public void Index_Should_Throw_When_IndexNotInRange(int index)
        {
            var list = Create();
            list.Add(Fixture.Create<T>());
            Assert.Throws<ArgumentOutOfRangeException>(() => list[index]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[index] = Fixture.Create<T>());
            
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void Add(int size)
        {
            var stack = Create();

            for (var i = 0; i < size; i++)
            {
                stack.Add(Fixture.Create<T>());
            }
           
            stack.Count.Should().Be(size);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        public void Insert_Should_Throw_When_IndexIsOutOfRange(int index)
        {
            var list = Create(Fixture.Create<T[]>());
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(index, Fixture.Create<T>()));
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Insert(int index)
        {
            var list = Create(Fixture.Create<T[]>());

            var @new = Fixture.Create<T>();
            list.Insert(index, @new);
            list.Count.Should().Be(4);
            list[index].Should().Be(@new);
        }
        
        [Fact]
        public void IndexOf()
        {
            var items = Fixture.Create<T[]>();
            var list = Create(items);
            list.Add(default);

            for (int i = 0; i < items.Length; i++)
            {
                list.IndexOf(items[i]).Should().Be(i);
            }
            
            list.IndexOf(default).Should().Be(items.Length);
        }
        
        [Fact]
        public void IndexOf_Should_ReturnNegative1_When_ItemNotFound()
        {
            var items = Fixture.Create<T[]>();
            var list = Create(items);
            list.IndexOf(Fixture.Create<T>()).Should().Be(-1);
        }
        
        [Fact]
        public void Remove()
        {
            var items = Fixture.Create<T[]>();
            var list = Create(items);
            list.Remove(items[1]).Should().BeTrue();
            list.Should().HaveCount(items.Length -1);
            
            list.Add(default);
            list.Remove(default).Should().BeTrue();
            list.Should().HaveCount(items.Length -1);
        }
        
        [Fact]
        public void RemoveAt_Should_ReturnFalse_When_ItemNotExist()
        {
            var items = Fixture.Create<T[]>();
            var list = Create(items);
            list.Remove(Fixture.Create<T>()).Should().BeFalse();
            list.Should().HaveCount(items.Length);
        }
        
        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void RemoveAt_Should_Throw_When_IndexIsInvalid(int index)
        {
            var items = Fixture.Create<T[]>();
            var list = Create(items);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(index)); 
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void RemoveAt(int index)
        {
            var items = Fixture.Create<T[]>();
            var list = Create(items);
            list.RemoveAt(index);
            list.Should().HaveCount(items.Length - 1);

            list.IndexOf(items[index]).Should().Be(-1);
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
                list.Add(Fixture.Create<T>());
            }
           
            list.Count.Should().Be(size);
            list.Clear();
            list.Count.Should().Be(0);
        }

        [Fact]
        public void Clone()
        {
            var array = Fixture.Create<T[]>();
            var list = Create();

            foreach (var item in array)
            {
                list.Add(item);
            }

            var clone = (IList<T>)list.Clone();
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
            var array = Fixture.Create<T[]>();
            var stack = Create();

            foreach (var item in array)
            {
                stack.Add(item);
            }

            var clone = stack.Clone();
            clone.Should().NotBeNull();
            (clone is IList<T>).Should().BeTrue();
            var cloneEnumerator = ((IList<T>)clone).GetEnumerator();
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

            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                stack.Add(value);
            }
            
            stack.Add(default);

            foreach (var value in values)
            {
                stack.Contains(value).Should().BeTrue();
            }

            stack.Contains(default).Should().BeTrue();
        }

        [Fact] 
        public void Contains_Should_BeFalse_When_DoesNotHaveValue()
        {
            var stack = Create();

            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                stack.Add(value);
            }
            
            stack.Contains(Fixture.Create<T>()).Should().BeFalse();
            stack.Contains(default).Should().BeFalse();
        }
        
        [Fact] 
        public void CopyTo()
        {
            var stack = Create();

            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                stack.Add(value);
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
                value.Should().Be((T)enumerable.Current);
            }
        }

        [Fact]
        public void Copy_Should_Throw()
        {
            var stack = Create();
            
            var values = Fixture.Create<T[]>();

            foreach (var value in values)
            {
                stack.Add(value);
            }
            
            Assert.Throws<ArgumentNullException>(() => stack.CopyTo(default!, values.Length));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.CopyTo(new T[10], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.CopyTo(new T[10], values.Length + 1));
            Assert.Throws<ArgumentException>(() => stack.CopyTo(new T[1], values.Length));
        }
    }
}