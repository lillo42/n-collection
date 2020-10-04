using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Set
{
    public abstract class AbstractSetTest<T> : AbstractionCollectionTest<T>
    {
        protected override T[] CreatArray(int size)
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
        
        private void Validate_ExceptWith(AbstractSet<T> set, IEnumerable<T> enumerable)
        {
            if (set.Count == 0 || Equals(enumerable, set))
            {
                set.ExceptWith(enumerable);
                set.Count.Should().Be(0);
            }
            else
            {
                var expected = new HashSet<T>(set, Comparer);
                foreach (var element in enumerable)
                {
                    expected.Remove(element);
                }

                set.ExceptWith(enumerable);
                set.Count.Should().Be(expected.Count);
                expected.SetEquals(set).Should().BeTrue();
            }
        }

        private void Validate_IntersectWith(AbstractSet<T> set, IEnumerable<T> enumerable)
        {
            if (set.Count == 0 || !enumerable.Any())
            {
                set.IntersectWith(enumerable);
                set.Count.Should().Be(0);
                set.Should().BeEmpty();
            }
            else if (Equals(set, enumerable))
            {
                var beforeOperation = new HashSet<T>(set, Comparer);
                set.IntersectWith(enumerable);
                beforeOperation.SetEquals(set).Should().BeTrue();
            }
            else
            {
                var comparer = GetIEqualityComparer();
                var expected = new HashSet<T>(Comparer);
                foreach (var value in set)
                {
                    if (enumerable.Contains(value, comparer))
                    {
                        expected.Add(value);
                    }
                }

                set.IntersectWith(enumerable);
                set.Count.Should().Be(expected.Count);
                expected.SetEquals(set).Should().BeTrue();
            }
        }

        private void Validate_IsProperSubsetOf(AbstractSet<T> set, IEnumerable<T> enumerable)
        {
            var setContainsValueNotInEnumerable = false;
            var enumerableContainsValueNotInSet = false;
            var comparer = GetIEqualityComparer();
            
            foreach (var value in set) // Every value in Set must be in Enumerable
            {
                if (!enumerable.Contains(value, comparer))
                {
                    setContainsValueNotInEnumerable = true;
                    break;
                }
            }
            
            foreach (var value in enumerable) // Enumerable must contain at least one value not in Set
            {
                if (!set.Contains(value, comparer))
                {
                    enumerableContainsValueNotInSet = true;
                    break;
                }
            }
            
            set.IsProperSubsetOf(enumerable).Should().Be(!setContainsValueNotInEnumerable && enumerableContainsValueNotInSet);
        }

        private void Validate_IsProperSupersetOf(AbstractSet<T> set, IEnumerable<T> enumerable)
        {
            var isProperSuperset = true;
            var setContainsElementsNotInEnumerable = false;
            var comparer = GetIEqualityComparer();
            
            foreach (var value in enumerable)
            {
                if (!set.Contains(value, comparer))
                {
                    isProperSuperset = false;
                    break;
                }
            }
            
            foreach (var value in set)
            {
                if (!enumerable.Contains(value, comparer))
                {
                    setContainsElementsNotInEnumerable = true;
                    break;
                }
            }
            
            isProperSuperset = isProperSuperset && setContainsElementsNotInEnumerable;
            set.IsProperSupersetOf(enumerable).Should().Be(isProperSuperset);
        }

        private void Validate_IsSubsetOf(AbstractSet<T> set, IEnumerable<T> enumerable)
        {
            var comparer = GetIEqualityComparer();
            foreach (var value in set)
            {
                if (!enumerable.Contains(value, comparer))
                {
                    Assert.False(set.IsSubsetOf(enumerable));
                    return;
                }
            }

            set.IsSubsetOf(enumerable).Should().BeTrue();
        }

        private void Validate_IsSupersetOf(AbstractSet<T> set, IEnumerable<T> enumerable)
        {
            var comparer = GetIEqualityComparer();
            foreach (var value in enumerable)
            {
                if (!set.Contains(value, comparer))
                {
                    Assert.False(set.IsSupersetOf(enumerable));
                    return;
                }
            }

            set.IsSupersetOf(enumerable).Should().BeTrue();
        }

        private void Validate_Overlaps(AbstractSet<T> set, IEnumerable<T> enumerable)
        {
            var comparer = GetIEqualityComparer();
            foreach (var value in enumerable)
            {
                if (set.Contains(value, comparer))
                {
                    Assert.True(set.Overlaps(enumerable));
                    return;
                }
            }
            
            
            Assert.False(set.Overlaps(enumerable));
        }

        private void Validate_SetEquals(AbstractSet<T> set, IEnumerable<T> enumerable)
        {
            var comparer = GetIEqualityComparer();
            foreach (var value in set)
            {
                if (!enumerable.Contains(value, comparer))
                {
                    set.SetEquals(enumerable).Should().BeFalse();
                    return;
                }
            }
            
            foreach (var value in enumerable)
            {
                if (!set.Contains(value, comparer))
                {
                    set.SetEquals(enumerable).Should().BeFalse();
                    return;
                }
            }
            
            set.SetEquals(enumerable).Should().BeTrue();
        }

        private void Validate_SymmetricExceptWith(AbstractSet<T> set, IEnumerable<T> enumerable)
        {
            var comparer = GetIEqualityComparer();
            var expected = new HashSet<T>(Comparer);
            foreach (var element in enumerable)
            {
                if (!set.Contains(element, comparer))
                {
                    expected.Add(element);
                }
            }

            foreach (var element in set)
            {
                if (!enumerable.Contains(element, comparer))
                {
                    expected.Add(element);
                }
            }

            set.SymmetricExceptWith(enumerable);
            set.Count.Should().Be(expected.Count);
            expected.SetEquals(set).Should().BeTrue();
        }

        private void Validate_UnionWith(ISet<T> set, IEnumerable<T> enumerable)
        {
            var comparer = GetIEqualityComparer();
            var expected = new HashSet<T>(set, Comparer);
            foreach (var element in enumerable)
            {
                if (!set.Contains(element, comparer))
                {
                    expected.Add(element);
                }
            }

            set.UnionWith(enumerable);
            set.Count.Should().Be(expected.Count);
            Assert.True(expected.SetEquals(set));
        }
        
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSetTest_Add_Duplicated_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateSet(array);

            foreach (var item in array)
            {
                collection.Add(item).Should().BeFalse();
            }

            collection.Should().HaveCount(size);
        }
        
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSetTest_TryAdd_Duplicated_Validity(int size)
        {
            var array = CreatArray(size);
            var collection = CreateSet(array);

            foreach (var item in array)
            {
                collection.TryAdd(item).Should().BeFalse();
            }

            collection.Should().HaveCount(size);
        }
        
        #region Set Function tests

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSetTest_Generic_NullEnumerableArgument(int count)
        {
            var set = CreateSet(CreatArray(count));
            Assert.Throws<ArgumentNullException>(() => set.ExceptWith(null!));
            Assert.Throws<ArgumentNullException>(() => set.IntersectWith(null!));
            Assert.Throws<ArgumentNullException>(() => set.IsProperSubsetOf(null!));
            Assert.Throws<ArgumentNullException>(() => set.IsProperSupersetOf(null!));
            Assert.Throws<ArgumentNullException>(() => set.IsSubsetOf(null!));
            Assert.Throws<ArgumentNullException>(() => set.IsSupersetOf(null!));
            Assert.Throws<ArgumentNullException>(() => set.Overlaps(null!));
            Assert.Throws<ArgumentNullException>(() => set.SetEquals(null!));
            Assert.Throws<ArgumentNullException>(() => set.SymmetricExceptWith(null!));
            Assert.Throws<ArgumentNullException>(() => set.UnionWith(null!));
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void AbstractSetTest_Generic_ExceptWith(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var set = CreateSet(CreatArray(setLength));
            var enumerable = CreateEnumerable(enumerableType, set, enumerableLength, numberOfMatchingElements, numberOfDuplicateElements);
            Validate_ExceptWith(set, enumerable);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void AbstractSetTest_Generic_IntersectWith(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var set = CreateSet(CreatArray(setLength));
            var enumerable = CreateEnumerable(enumerableType, set, enumerableLength, numberOfMatchingElements, numberOfDuplicateElements);
            Validate_IntersectWith(set, enumerable);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void AbstractSetTest_Generic_IsProperSubsetOf(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var set = CreateSet(CreatArray(setLength));
            var enumerable = CreateEnumerable(enumerableType, set, enumerableLength, numberOfMatchingElements, numberOfDuplicateElements);
            Validate_IsProperSubsetOf(set, enumerable);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void AbstractSetTest_Generic_IsProperSupersetOf(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var set = CreateSet(CreatArray(setLength));
            var enumerable = CreateEnumerable(enumerableType, set, enumerableLength, numberOfMatchingElements, numberOfDuplicateElements);
            Validate_IsProperSupersetOf(set, enumerable);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void AbstractSetTest_Generic_IsSubsetOf(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var set = CreateSet(CreatArray(setLength));
            var enumerable = CreateEnumerable(enumerableType, set, enumerableLength, numberOfMatchingElements, numberOfDuplicateElements);
            Validate_IsSubsetOf(set, enumerable);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void AbstractSetTest_Generic_IsSupersetOf(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var set = CreateSet(CreatArray(setLength));
            var enumerable = CreateEnumerable(enumerableType, set, enumerableLength, numberOfMatchingElements, numberOfDuplicateElements);
            Validate_IsSupersetOf(set, enumerable);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void AbstractSetTest_Generic_Overlaps(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var set = CreateSet(CreatArray(setLength));
            var enumerable = CreateEnumerable(enumerableType, set, enumerableLength, numberOfMatchingElements, numberOfDuplicateElements);
            Validate_Overlaps(set, enumerable);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void AbstractSetTest_Generic_SetEquals(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var set = CreateSet(CreatArray(setLength));
            var enumerable = CreateEnumerable(enumerableType, set, enumerableLength, numberOfMatchingElements, numberOfDuplicateElements);
            Validate_SetEquals(set, enumerable);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void AbstractSetTest_Generic_SymmetricExceptWith(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var set = CreateSet(CreatArray(setLength));
            var enumerable = CreateEnumerable(enumerableType, set, enumerableLength, numberOfMatchingElements, numberOfDuplicateElements);
            Validate_SymmetricExceptWith(set, enumerable);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void AbstractSetTest_Generic_UnionWith(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var set = CreateSet(CreatArray(setLength));
            var enumerable = CreateEnumerable(enumerableType, set, enumerableLength, numberOfMatchingElements, numberOfDuplicateElements);
            Validate_UnionWith(set, enumerable);
        }

        #endregion
        
        #region Set Function tests on itself

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSet_Generic_ExceptWith_Itself(int size)
        {
            var set = CreateSet(CreatArray(size));
            Validate_ExceptWith(set, set);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))] 
        public void AbstractSet_Generic_IntersectWith_Itself(int size)
        {
            var set = CreateSet(CreatArray(size));
            Validate_IntersectWith(set, set);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSet_Generic_IsProperSubsetOf_Itself(int size)
        {
            var set = CreateSet(CreatArray(size));
            Validate_IsProperSubsetOf(set, set);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSet_Generic_IsProperSupersetOf_Itself(int size)
        {
            var set = CreateSet(CreatArray(size));
            Validate_IsProperSupersetOf(set, set);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSet_Generic_IsSubsetOf_Itself(int size)
        {
            var set = CreateSet(CreatArray(size));
            Validate_IsSubsetOf(set, set);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSet_Generic_IsSupersetOf_Itself(int size)
        {
            var set = CreateSet(CreatArray(size));
            Validate_IsSupersetOf(set, set);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSet_Generic_Overlaps_Itself(int size)
        {
            var set = CreateSet(CreatArray(size));
            Validate_Overlaps(set, set);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSet_Generic_SetEquals_Itself(int size)
        {
            var set = CreateSet(CreatArray(size));
            Assert.True(set.SetEquals(set));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSet_Generic_SymmetricExceptWith_Itself(int size)
        {
            var set = CreateSet(CreatArray(size));
            Validate_SymmetricExceptWith(set, set);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void AbstractSet_Generic_UnionWith_Itself(int size)
        {
            var set = CreateSet(CreatArray(size));
            Validate_UnionWith(set, set);
        }

        #endregion
    }
}
