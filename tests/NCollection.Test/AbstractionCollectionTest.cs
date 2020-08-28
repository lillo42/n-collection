using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace NCollection.Test
{
    public abstract class AbstractionCollectionTest<T>
    {
        protected Fixture Fixture { get; } = new Fixture();
        
        protected virtual T Create()
        {
            return Fixture.Create<T>();
        }

        protected virtual T[] CreateArray(int size)
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
            var array = CreateArray(size);
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
            var array = CreateArray(size);
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
        
        #region Add All

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_AddAll_Validity(int size)
        {
            var array = CreateArray(size);
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
            var array = CreateArray(size);
            var collection = CreateCollection();

            collection.AddAll(array).Should().BeTrue();
            collection.Should().HaveCount(size);
            
            collection.Clear();
            
            collection.AddAll(array).Should().BeTrue();
            collection.Should().HaveCount(size);
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
            var collection = CreateCollection(CreateArray(size));
            collection.Should().HaveCount(size);
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
            var array = CreateArray(size);
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
            var array = CreateArray(size);
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
            var array = CreateArray(size);
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
            var array = CreateArray(size);
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
            var array = CreateArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);

            collection.ContainsAll(CreateArray(size)).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_ContainsAll_Invalidity_AfterClear(int size)
        {
            var array = CreateArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            collection.Clear();
            
            collection.ContainsAll(array).Should().BeFalse();
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
            var array = CreateArray(size);
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
            var array = CreateArray(size);
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
            var array = CreateArray(size);
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
            var array = CreateArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            
            collection.RemoveAll(CreateArray(size)).Should().BeFalse();

            collection.Should().HaveCount(size);
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
            var array = CreateArray(size);
            var collection = CreateCollection(array);

            collection.ToArray().Should().BeEquivalentTo(array);
        }

        #endregion
    }
}