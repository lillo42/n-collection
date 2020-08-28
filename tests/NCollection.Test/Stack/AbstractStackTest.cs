using System.Linq;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Stack
{
    public abstract class AbstractStackTest<T> : AbstractionCollectionTest<T>
    {
        protected abstract AbstractStack<T> CreateStack();
        protected override AbstractCollection<T> CreateCollection()
        {
            return CreateStack();
        }

        #region Remove

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public override void AbstractionCollectionTest_Remove_Validity(int size)
        {
            var array = CreateArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            
            foreach (var item in array.Reverse())
            {
                collection.Remove(item).Should().BeTrue();
            }
            
            collection.Should().BeEmpty();
        }

        #endregion

        #region Remove All

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public override void AbstractionCollectionTest_RemoveAll_Validity(int size)
        {
            var array = CreateArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            collection.RemoveAll(array.Reverse()).Should().BeTrue();
            collection.Should().BeEmpty();
        }

        #endregion

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public override void AbstractionCollectionTest_ToArray(int size)
        {
            var array = CreateArray(size);
            var collection = CreateCollection(array);

            collection.ToArray().Should().BeEquivalentTo(array.Reverse());
        }
    }
}