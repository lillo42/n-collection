using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Set
{
    public abstract class AbstractSetTest<T> : AbstractionCollectionTest<T>
    {
        protected override T[] CreateAValidArray(int size)
        {
            var result = new T[size];
            var index = 0;
            while (index < size)
            {
                var item = Create();
                if (!result.Contains(item))
                {
                    result[index] = item;
                    index++;
                }
            }

            return result;
        }
        protected abstract AbstractSet<T> CreateSet();
        protected abstract AbstractSet<T> CreateSet(IEnumerable<T> source);
        protected override AbstractCollection<T> CreateCollection()
        {
            return CreateSet();
        }
        protected override AbstractCollection<T> CreateCollection(IEnumerable<T> array)
        {
            return CreateSet(array);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractSetTest_Add_Duplicated_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateSet(array);

            foreach (var item in array)
            {
                collection.Add(item).Should().BeFalse();
            }

            collection.Should().HaveCount(size);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractSetTest_TryAdd_Duplicated_Validity(int size)
        {
            var array = CreateAValidArray(size);
            var collection = CreateSet(array);

            foreach (var item in array)
            {
                collection.TryAdd(item).Should().BeFalse();
            }

            collection.Should().HaveCount(size);
        }
        
        private void Validate_ExceptWith(AbstractSet<T> set, IEnumerable<T> enumerable)
        {
            if (set.Count == 0 || Equals(enumerable, set))
            {
                set.ExceptWith(enumerable);
                set.Count.Should().Be(0);
            }
            else
            {
                var expected = CreateSet(set);
                
                foreach (var element in enumerable)
                {
                    expected.Remove(element);
                }

                set.ExceptWith(enumerable);
                expected.Count.Should().Be( set.Count);
                expected.SetEquals(set).Should().BeTrue();
            }
        }
        
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSet_ExceptWith_Itself(int size)
        {
            var set = CreateSet(CreateAValidArray(size));
            Validate_ExceptWith(set, set);
        }
    }
}
