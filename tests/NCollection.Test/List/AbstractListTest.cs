using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.List
{
    public abstract class AbstractListTest<T> : AbstractionCollectionTest<T>
    {
        protected abstract AbstractList<T> CreateList();

        protected virtual AbstractList<T> CreateList(IEnumerable<T> source)
        {
            var list = CreateList();
         
            list.AddAll(source);

            return list;
        }

        protected override AbstractCollection<T> CreateCollection()
        {
            return CreateList();
        }

        protected override AbstractCollection<T> CreateCollection(IEnumerable<T> array)
        {
            return CreateList(array);
        }

        #region Index Of

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractListTest_IndexOf_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);

            collection.Should().HaveCount(size);

            for (var i = 0; i < array.Length; i++)
            {
                collection.IndexOf(array[i]).Should().Be(i);
            }
        }
        
        [Fact]
        public void AbstractListTest_IndexOf_Invalid()
        {
            var collection = CreateList(CreatArray(10));

            collection.IndexOf(Create()).Should().Be(-1);
        }

        #endregion
        
        #region Last Index Of

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractListTest_LastIndexOf_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);
            collection.AddAll(array);
            collection.Should().HaveCount(size * 2);

            var index = size - 1;
            for (var i = collection.Count - 1; i >  array.Length; i--)
            {
                collection.LastIndexOf(array[index]).Should().Be(i);
                index--;
            }
        }
        
        [Fact]
        public void AbstractListTest_LastIndexOf_Invalid()
        {
            var collection = CreateList(CreatArray(10));

            collection.LastIndexOf(Create()).Should().Be(-1);
        }

        #endregion
        
        #region Add at Index

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractListTest_Add_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);

            var position = size / 2;
            var item = Create();
            collection.Add(position, item);
            collection[position].Should().Be(item);
            collection.Should().HaveCount(size + 1);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractListTest_Add_AfterClear(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);

            collection.Should().HaveCount(size);
            
            collection.Clear();
            
            collection.AddAll(array);
            
            var position = size / 2;
            var item = Create();
            collection.Add(position, item);
            
            collection.Should().HaveCount(size + 1);
        }

        #endregion
        
        #region AddAll at Index

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractListTest_AddAll_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);

            var position = size / 2;
            var items = CreatArray(size);
            collection.AddAll(position, items);

            for (var i = 0; i < size; i++)
            {
                collection[i + position].Should().Be(items[i]);
            }
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractListTest_AddAll_AfterClear(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);

            collection.Clear();
            collection.AddAll(array);
            
            var position = size / 2;
            var items = CreatArray(size);
            collection.AddAll(position, items);

            for (var i = 0; i < size; i++)
            {
                collection[i + position].Should().Be(items[i]);
            }
        }

        #endregion
        
        #region Remove at Index

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractListTest_RemoveAt_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);

            var position = size / 2;
            var item = collection[position];
            collection.RemoveAt(position);
            collection.Should().HaveCount(size - 1);
            if (position > 0)
            {
                collection[position].Should().NotBe(item);
            }
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractListTest_RemoveAt_AfterClear(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);
            
            collection.Clear();
            collection.AddAll(array);
            
            var position = size / 2;
            var item = collection[position];
            collection.RemoveAt(position);
            collection.Should().HaveCount(size - 1);
            if (position > 0)
            {
                collection[position].Should().NotBe(item);
            }
        }

        #endregion

        #region Access Index
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractListTest_Index_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);
            
            var position = size / 2;
            collection[position].Should().Be(array[position]);
            var item = Create();
            collection[position] = item;
            collection[position].Should().Be(item);
        }
        

        #endregion
        
        #region Range
        
        [Theory]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractListTest_Range_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);
            var range = collection[..];
            
            for (var i = 0; i < size; i++)
            {
                range[i].Should().Be(collection[i]);
            }
        }
        
        [Theory]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractListTest_Range_Half_ToEnd(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);
            var position = size / 2;
            var range = collection[position..];

            range.Count.Should().Be(size - position);
            for (var i = 0; i < position; i++)
            {
                range[i].Should().Be(collection[i + position]);
            }
        }
        
        [Theory]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractListTest_Range_0_To_Half(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);
            var position = size / 2;
            var range = collection[..position];
            
            for (var i = 0; i < position; i++)
            {
                range[i].Should().Be(collection[i]);
            }
        }
        
        [Theory]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractListTest_Range_Exclude_Board(int size)
        {
            var array = CreatArray(size);
            var collection = CreateList(array);
            var range = collection[1..(size - 1)];
            
            range.Count.Should().Be(size - 2);

            for (var i = 0; i < range.Count; i++)
            {
                range[i].Should().Be(collection[i + 1]);
            }
        }
        

        #endregion

    }
}