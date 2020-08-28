using System;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace NCollection.Test
{
    public abstract class AbstractionCollectionTest<T>
    {
        protected Fixture Fixture { get; } = new Fixture();
        protected virtual bool IsReadOnly => false;
        
        protected virtual T Create()
        {
            return Fixture.Create<T>();
        }

        protected virtual T[] CreateAValidArray(int size)
        {
            var result = new T[size];
            for (var i = 0; i < size; i++)
            {
                result[i] = Create();
            }

            return result;
        }
        
        protected abstract AbstractCollection<T> CreateCollection();
        
        protected virtual AbstractCollection<T> CreateCollection(T[] array)
        {
            var collection = CreateCollection();
            collection.AddAll(array);
            return collection;
        }
        
        #region Add

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_Add_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_Add_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }
            
            collection.Should().HaveCount(size);
            
            collection.Clear();
            
            foreach (var item in array)
            {
                collection.Add(item);
            }
            
            collection.Should().HaveCount(size);
        }

        #endregion
        
        #region TryAdd

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_TryAdd_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.TryAdd(item).Should().BeTrue();
            }

            collection.Should().HaveCount(size);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_TryAdd_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.TryAdd(item).Should().BeTrue();
            }
            
            collection.Should().HaveCount(size);
            
            collection.Clear();
            
            foreach (var item in array)
            {
                collection.Add(item);
            }
            
            collection.Should().HaveCount(size);
        }

        #endregion
        
        #region Add All

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_AddAll_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            collection.AddAll(array).Should().BeTrue();
            
            collection.Should().HaveCount(size);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_AddAll_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            collection.AddAll(array).Should().BeTrue();
            collection.Should().HaveCount(size);
            
            collection.Clear();
            
            collection.AddAll(array).Should().BeTrue();
            collection.Should().HaveCount(size);
        }
        
        [Fact]
        public void AbstractionCollectionTest_AddAll_Throw()
        {
            var collection = CreateCollection();

            Assert.Throws<ArgumentNullException>(() => collection.AddAll(null!));
        }
        #endregion

        #region Count

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_Count_Validity(int size)
        {
            var collection = CreateCollection(CreateAValidArray(size));
            collection.Should().HaveCount(size);
        }

        #endregion

        #region Is Read Only
        [Fact]
        public void AbstractionCollectionTest_ReadOnly_Validity()
        {
            var collection = CreateCollection();
            collection.IsReadOnly.Should().Be(IsReadOnly);
        }

        #endregion

        #region Contains

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_Contains_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            
            foreach (var item in array)
            {
                collection.Contains(item).Should().BeTrue();
            }
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_Contains_Invalidity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);

            collection.Contains(Create()).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_Contains_Invalidity_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            collection.Clear();
            
            foreach (var item in array)
            {
                collection.Contains(item).Should().BeFalse();
            }
        }

        [Fact]
        public void AbstractionCollectionTest_Contains_Throw()
        {
            var collection = CreateCollection();

            Assert.Throws<ArgumentNullException>(() => collection.Contains(Create(), null!));
        }
        
        #endregion
        
        #region Contains All

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_ContainsAll_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            
            collection.ContainsAll(array).Should().BeTrue();
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_ContainsAll_Invalidity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);

            collection.ContainsAll(CreateAValidArray(size)).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_ContainsAll_Invalidity_AfterClear(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            collection.Clear();
            
            collection.ContainsAll(array).Should().BeFalse();
        }
        
        [Fact]
        public void AbstractionCollectionTest_ContainsAll_Throw()
        {
            var collection = CreateCollection();
            
            Assert.Throws<ArgumentNullException>(() => collection.ContainsAll(null!));
            Assert.Throws<ArgumentNullException>(() => collection.ContainsAll(null!, null!));
            Assert.Throws<ArgumentNullException>(() => collection.ContainsAll(CreateAValidArray(1), null!));
        }

        #endregion
        
        #region Remove

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractionCollectionTest_Remove_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            
            foreach (var item in array)
            {
                collection.Remove(item).Should().BeTrue();
            }
            
            collection.Should().BeEmpty();
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_Remove_Invalidity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            
            collection.Remove(Create()).Should().BeFalse();

            collection.Should().HaveCount(size);
        }
        #endregion
        
        #region Remove All

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractionCollectionTest_RemoveAll_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            collection.RemoveAll(array).Should().BeTrue();
            collection.Should().BeEmpty();
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_RemoveAll_Invalidity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            
            collection.RemoveAll(CreateAValidArray(size)).Should().BeFalse();

            collection.Should().HaveCount(size);
        }
        
        [Fact]
        public void AbstractionCollectionTest_RemoveAll_Throw()
        {
            var collection = CreateCollection();
            
            Assert.Throws<ArgumentNullException>(() => collection.RemoveAll(null!));
        }
        #endregion

        #region ToArray

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractionCollectionTest_ToArray(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateCollection(array);

            collection.ToArray().Should().BeEquivalentTo(array);
        }
        
        #endregion
        
        #region ToArray

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractionCollectionTest_CopyTo_Valid(int size)
        {
            var collection = CreateCollection(CreateAValidArray(size));

            var ret = new T[size];
            collection.CopyTo(ret, 0);

            var i = 0;
            foreach (var item in collection)
            {
                item.Should().BeEquivalentTo(ret[i]);
                i++;
            }

            i.Should().Be(size);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractionCollectionTest_CopyTo_Throw(int size)
        {
            var collection = CreateCollection(CreateAValidArray(size));

            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null!, 0));
            
            var ret = new T[size];
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(ret, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(ret, collection.Count + 1));
        }
        
        #endregion
    }
}