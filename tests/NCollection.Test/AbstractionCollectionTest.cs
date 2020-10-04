using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace NCollection.Test
{
    public abstract class AbstractionCollectionTest<T>
    {
        public static IEnumerable<object[]> ValidCollectionSizes()
        {
            yield return new object[] { 1 };
            yield return new object[] { 10 };
            yield return new object[] { 75 };
            yield return new object[] { 100 };
        }
        
        public enum EnumerableType
        {
            HashSet,
            //SortedSet,
            ArrayList,
            LinkedList,
            ArrayQueue,
            LinkedQueue,
            Lazy,
        }
        
        protected virtual IComparer<T> Comparer => Comparer<T>.Default;

        protected virtual IEqualityComparer<T> GetIEqualityComparer() => new DefaultIEqualityComparer(Comparer);


        /// <summary>
        /// Helper function to create an enumerable fulfilling the given specific parameters. The function will
        /// create an enumerable of the desired type using the Default constructor for that type and then add values
        /// to it until it is full. It will begin by adding the desired number of matching and duplicate elements,
        /// followed by random (deterministic) elements until the desired count is reached.
        /// </summary>
        protected IEnumerable<T> CreateEnumerable(EnumerableType type, IEnumerable<T> enumerableToMatchTo, int count, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            switch (type)
            {
                case EnumerableType.HashSet:
                    return CreateHashSet(enumerableToMatchTo, count, numberOfMatchingElements);
                case EnumerableType.ArrayList:
                    return CreateArrayList(enumerableToMatchTo, count, numberOfMatchingElements, numberOfDuplicateElements);
                case EnumerableType.LinkedList:
                    return CreateLinkedList(enumerableToMatchTo, count, numberOfMatchingElements, numberOfDuplicateElements);
                case EnumerableType.ArrayQueue:
                    return CreateArrayQueue(enumerableToMatchTo, count, numberOfMatchingElements, numberOfDuplicateElements);
                case EnumerableType.LinkedQueue:
                    return CreateLinkedQueue(enumerableToMatchTo, count, numberOfMatchingElements, numberOfDuplicateElements);
                case EnumerableType.Lazy:
                    return CreateLazyEnumerable(enumerableToMatchTo, count, numberOfMatchingElements, numberOfDuplicateElements);
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Helper function to create an List fulfilling the given specific parameters. The function will
        /// create an List and then add values
        /// to it until it is full. It will begin by adding the desired number of matching,
        /// followed by random (deterministic) elements until the desired count is reached.
        /// </summary>
        protected IEnumerable<T> CreateArrayList(IEnumerable<T> enumerableToMatchTo, int count, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            
            var list = new ArrayList<T>(count == 0 ? 4: count);
            var duplicateAdded = 0;
            
            ArrayList<T> match = null;

            // Add Matching elements
            if (enumerableToMatchTo != null)
            {
                match = new ArrayList<T>(enumerableToMatchTo);
                
                for (var i = 0; i < numberOfMatchingElements; i++)
                {
                    list.Add(match[i]);
                    while (duplicateAdded++ < numberOfDuplicateElements)
                    {
                        list.Add(match[i]);
                    }
                }
            }

            // Add elements to reach the desired count
            while (list.Count < count)
            {
                var toAdd = Create();
                while (list.Contains(toAdd) || (match != null && match.Contains(toAdd))) // Don't want any unexpectedly duplicate values
                {
                    toAdd = Create();
                }

                list.Add(toAdd);
                while (duplicateAdded++ < numberOfDuplicateElements)
                {
                    list.Add(toAdd);
                }
            }

            // Validate that the Enumerable fits the guidelines as expected
            if (match != null)
            {
                var actualMatchingCount = 0;
                foreach (var lookingFor in match)
                {
                    actualMatchingCount += list.Contains(lookingFor) ? 1 : 0;
                }

                actualMatchingCount.Should().Be(numberOfMatchingElements);
            }

            return list;
        }

        /// <summary>
        /// Helper function to create an List fulfilling the given specific parameters. The function will
        /// create an List and then add values
        /// to it until it is full. It will begin by adding the desired number of matching,
        /// followed by random (deterministic) elements until the desired count is reached.
        /// </summary>
        protected IEnumerable<T> CreateLinkedList(IEnumerable<T> enumerableToMatchTo, int count, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var list = new LinkedList<T>();
            var duplicateAdded = 0;
            
            LinkedList<T> match = null;

            // Add Matching elements
            if (enumerableToMatchTo != null)
            {
                match = new LinkedList<T>(enumerableToMatchTo);
                
                for (var i = 0; i < numberOfMatchingElements; i++)
                {
                    list.Add(match[i]);
                    while (duplicateAdded++ < numberOfDuplicateElements)
                    {
                        list.Add(match[i]);
                    }
                }
            }

            // Add elements to reach the desired count
            while (list.Count < count)
            {
                var toAdd = Create();
                while (list.Contains(toAdd) || (match != null && match.Contains(toAdd))) // Don't want any unexpectedly duplicate values
                {
                    toAdd = Create();
                }

                list.Add(toAdd);
                while (duplicateAdded++ < numberOfDuplicateElements)
                {
                    list.Add(toAdd);
                }
            }

            // Validate that the Enumerable fits the guidelines as expected
            if (match != null)
            {
                var actualMatchingCount = 0;
                foreach (var lookingFor in match)
                {
                    actualMatchingCount += list.Contains(lookingFor) ? 1 : 0;
                }

                actualMatchingCount.Should().Be(numberOfMatchingElements);
            }

            return list;
        }
        
        
        protected IEnumerable<T> CreateLazyEnumerable(IEnumerable<T> enumerableToMatchTo, int count, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var list = CreateArrayList(enumerableToMatchTo, count, numberOfMatchingElements, numberOfDuplicateElements);
            return list.Select(item => item);
        }
        
        /// <summary>
        /// Helper function to create a Queue fulfilling the given specific parameters. The function will
        /// create an Queue and then add values
        /// to it until it is full. It will begin by adding the desired number of matching,
        /// followed by random (deterministic) elements until the desired count is reached.
        /// </summary>
        protected IEnumerable<T> CreateArrayQueue(IEnumerable<T> enumerableToMatchTo, int count, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var queue = new ArrayQueue<T>(count == 0 ? 4 : count);
            var duplicateAdded = 0;
            ArrayList<T> match = null;

            // Enqueue Matching elements
            if (enumerableToMatchTo != null)
            {
                match = new ArrayList<T>(enumerableToMatchTo);
                for (var i = 0; i < numberOfMatchingElements; i++)
                {
                    queue.Enqueue(match[i]);
                    while (duplicateAdded++ < numberOfDuplicateElements)
                    {
                        queue.Enqueue(match[i]);
                    }
                }
            }

            // Enqueue elements to reach the desired count
            while (queue.Count < count)
            {
                var toEnqueue = Create();
                while (queue.Contains(toEnqueue) || (match != null && match.Contains(toEnqueue))) // Don't want any unexpectedly duplicate values
                {
                    toEnqueue = Create();
                }

                queue.Enqueue(toEnqueue);
                while (duplicateAdded++ < numberOfDuplicateElements)
                {
                    queue.Enqueue(toEnqueue);
                }
            }

           
            if (match != null)
            {
                var actualMatchingCount = 0;
                foreach (var lookingFor in match)
                {
                    actualMatchingCount += queue.Contains(lookingFor) ? 1 : 0;
                }

                actualMatchingCount.Should().Be(numberOfMatchingElements);
            }

            return queue;
        }

        /// <summary>
        /// Helper function to create a Queue fulfilling the given specific parameters. The function will
        /// create an Queue and then add values
        /// to it until it is full. It will begin by adding the desired number of matching,
        /// followed by random (deterministic) elements until the desired count is reached.
        /// </summary>
        protected IEnumerable<T> CreateLinkedQueue(IEnumerable<T> enumerableToMatchTo, int count, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            var queue = new LinkedQueue<T>();
            var duplicateAdded = 0;
            ArrayList<T> match = null;

            // Enqueue Matching elements
            if (enumerableToMatchTo != null)
            {
                match = new ArrayList<T>(enumerableToMatchTo);
                for (var i = 0; i < numberOfMatchingElements; i++)
                {
                    queue.Enqueue(match[i]);
                    while (duplicateAdded++ < numberOfDuplicateElements)
                    {
                        queue.Enqueue(match[i]);
                    }
                }
            }

            // Enqueue elements to reach the desired count
            while (queue.Count < count)
            {
                var toEnqueue = Create();
                while (queue.Contains(toEnqueue) || (match != null && match.Contains(toEnqueue))) // Don't want any unexpectedly duplicate values
                {
                    toEnqueue = Create();
                }

                queue.Enqueue(toEnqueue);
                while (duplicateAdded++ < numberOfDuplicateElements)
                {
                    queue.Enqueue(toEnqueue);
                }
            }

           
            if (match != null)
            {
                var actualMatchingCount = 0;
                foreach (var lookingFor in match)
                {
                    actualMatchingCount += queue.Contains(lookingFor) ? 1 : 0;
                }

                actualMatchingCount.Should().Be(numberOfMatchingElements);
            }

            return queue;
        }

        /// <summary>
        /// Helper function to create an HashSet fulfilling the given specific parameters. The function will
        /// create an HashSet using the Comparer constructor and then add values
        /// to it until it is full. It will begin by adding the desired number of matching,
        /// followed by random (deterministic) elements until the desired count is reached.
        /// </summary>
        protected IEnumerable<T> CreateHashSet(IEnumerable<T> enumerableToMatchTo, int count, int numberOfMatchingElements)
        {
            var set = new HashSet<T>(Comparer);
            ArrayList<T> match = null;

            // Add Matching elements
            if (enumerableToMatchTo != null)
            {
                match = new ArrayList<T>(enumerableToMatchTo);
                for (var i = 0; i < numberOfMatchingElements; i++)
                {
                    set.Add(match[i]);
                }
            }

            // Add elements to reach the desired count
            while (set.Count < count)
            {
                var toAdd = Create();
                while (set.Contains(toAdd) || (match != null && match.Contains(toAdd, GetIEqualityComparer()))) // Don't want any unexpectedly duplicate values
                {
                    toAdd = Create();
                }

                set.Add(toAdd);
            }

            // Validate that the Enumerable fits the guidelines as expected
            if (match != null)
            {
                var actualMatchingCount = 0;
                foreach (var lookingFor in match)
                {
                    actualMatchingCount += set.Contains(lookingFor) ? 1 : 0;
                }

                actualMatchingCount.Should().Be(numberOfMatchingElements);
            }

            return set;
        }

        
        /// <summary>
        /// MemberData to be passed to tests that take an IEnumerable{T}. This method returns every permutation of
        /// EnumerableType to test on (e.g. HashSet, Queue), and size of set to test with (e.g. 0, 1, etc.).
        /// </summary>
        public static IEnumerable<object[]> EnumerableTestData()
        {
            foreach (var collectionSizeArray in ValidCollectionSizes())
            {
                foreach (EnumerableType enumerableType in Enum.GetValues(typeof(EnumerableType)))
                {
                    var count = (int)collectionSizeArray[0];
                    yield return new object[] { enumerableType, count, 0, 0, 0 };                       // Empty Enumerable
                    yield return new object[] { enumerableType, count, count + 1, 0, 0 };               // Enumerable that is 1 larger

                    if (count >= 1)
                    {
                        yield return new object[] { enumerableType, count, count, 0, 0 };               // Enumerable of the same size
                        yield return new object[] { enumerableType, count, count - 1, 0, 0 };           // Enumerable that is 1 smaller
                        yield return new object[] { enumerableType, count, count, 1, 0 };               // Enumerable of the same size with 1 matching element
                        yield return new object[] { enumerableType, count, count + 1, 1, 0 };           // Enumerable that is 1 longer with 1 matching element
                        yield return new object[] { enumerableType, count, count, count, 0 };           // Enumerable with all elements matching
                        yield return new object[] { enumerableType, count, count + 1, count, 0 };       // Enumerable with all elements matching plus one extra
                    }

                    if (count >= 2)
                    {
                        yield return new object[] { enumerableType, count, count - 1, 1, 0 };           // Enumerable that is 1 smaller with 1 matching element
                        yield return new object[] { enumerableType, count, count + 2, 2, 0 };           // Enumerable that is 2 longer with 2 matching element
                        yield return new object[] { enumerableType, count, count - 1, count - 1, 0 };   // Enumerable with all elements matching minus one
                        yield return new object[] { enumerableType, count, count, 2, 0 };               // Enumerable of the same size with 2 matching element
                        if ((enumerableType == EnumerableType.ArrayList || enumerableType == EnumerableType.ArrayQueue))
                        {
                            yield return new object[] { enumerableType, count, count, 0, 1 };           // Enumerable with 1 element duplicated
                        }
                    }

                    if (count >= 3)
                    {
                        if ((enumerableType == EnumerableType.ArrayList || enumerableType == EnumerableType.ArrayQueue))
                        {
                            yield return new object[] { enumerableType, count, count, 0, 1 };           // Enumerable with all elements duplicated
                        }

                        yield return new object[] { enumerableType, count, count - 1, 2, 0 };           // Enumerable that is 1 smaller with 2 matching elements
                    }
                }
            }
        }
        
        protected Fixture Fixture { get; } = new Fixture();
        protected virtual bool IsReadOnly => false;
        protected virtual bool ContainsInitialCapacity => false;

        protected virtual T Create()
        {
            return Fixture.Create<T>();
        }

        protected virtual T[] CreatArray(int size)
        {
            var result = new T[size];
            for (var i = 0; i < size; i++)
            {
                result[i] = Create();
            }

            return result;
        }
        
        protected abstract AbstractCollection<T> CreateCollection();
        protected abstract AbstractCollection<T> CreateCollection(int size);
        protected abstract AbstractCollection<T> CreateCollection(int size, IEnumerable<T> source);
        
        protected virtual AbstractCollection<T> CreateCollection(IEnumerable<T> array)
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
            var array = CreatArray(size);
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
            var array = CreatArray(size);
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
            collection.Count.Should().Be(size);
        }

        #endregion
        
        #region TryAdd

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractionCollectionTest_TryAdd_Validity(int size)
        {
            var array = CreatArray(size);
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
        public virtual void AbstractionCollectionTest_TryAdd_AfterClear(int size)
        {
            var array = CreatArray(size);
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
            var array = CreatArray(size);
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
            var array = CreatArray(size);
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
            var collection = CreateCollection(CreatArray(size));
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
            var array = CreatArray(size);
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
            var array = CreatArray(size);
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
            var array = CreatArray(size);
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
            var array = CreatArray(size);
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
            var array = CreatArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);

            collection.ContainsAll(CreatArray(size)).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_ContainsAll_Invalidity_AfterClear(int size)
        {
            var array = CreatArray(size);
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
            var array = CreatArray(size);
            var collection = CreateCollection(array);
            
            collection.Should().HaveCount(size);
            
            foreach (var item in array)
            {
                collection.Remove(item).Should().BeTrue();
                collection.Contains(item).Should().BeFalse();
            }
            
            collection.Should().BeEmpty();
        }
        
        [Fact]
        public virtual void AbstractionCollectionTest_Remove_Invalidity()
        {
            var collection = CreateCollection();
            collection.Remove(Create()).Should().BeFalse();
            collection.Add(Create());
            collection.Remove(default).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractionCollectionTest_Remove_AfterClear_Invalidity(int size)
        {
            var collection = CreateCollection(CreatArray(size));
            collection.Clear();
            collection.Remove(Create()).Should().BeFalse();
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_Remove_ItemNotExist(int size)
        {
            var array = CreatArray(size);
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
            var array = CreatArray(size);
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
            var array = CreatArray(size);
            var collection = CreateCollection();

            foreach (var item in array)
            {
                collection.Add(item);
            }

            collection.Should().HaveCount(size);
            
            collection.RemoveAll(CreatArray(size)).Should().BeFalse();

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
            var array = CreatArray(size);
            var collection = CreateCollection(array);

            collection.ToArray().Should().BeEquivalentTo(array);
        }
        
        #endregion
        
        #region Copy To

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public virtual void AbstractionCollectionTest_CopyTo_Valid(int size)
        {
            var collection = CreateCollection(CreatArray(size));

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
            var collection = CreateCollection(CreatArray(size));

            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null!, 0));
            
            var ret = new T[size];
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(ret, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(ret, collection.Count + 1));
        }
        
        #endregion

        #region Constructor
        
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AbstractionCollectionTest_Constructor_Invalid_InitialCapacity(int size)
        {
            if (ContainsInitialCapacity)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => CreateCollection(size));
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_Constructor_Valid_InitialCapacity(int size)
        {
            if (ContainsInitialCapacity)
            {
                var stack = CreateCollection(size);
                stack.Add(Create());
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AbstractionCollectionTest_Constructor_Invalid_InitialCapacity_Valid_Source(int size)
        {
            if (ContainsInitialCapacity)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => CreateCollection(size, new T[0]));
            }
        }
        
        [Fact]
        public void AbstractionCollectionTest_Constructor_Valid_InitialCapacity_Valid_Source()
        {
            if (ContainsInitialCapacity)
            {
                Assert.Throws<ArgumentNullException>(() => CreateCollection(1, null!));
            }
        }

        [Fact]
        public void AbstractionCollectionTest_Constructor_Invalid_InitialCapacity_Valid_Source_Enumerable()
        {
            if (ContainsInitialCapacity)
            {
                var array = CreatArray(25);
                var collection = CreateCollection(25, Values());
                collection.Count.Should().Be(25);

                IEnumerable<T> Values()
                {
                    foreach (var item in array)
                    {
                        yield return item;
                    }
                }
            }
        }

        [Fact]
        public void AbstractionCollectionTest_Constructor_Invalid_InitialCapacity_Valid_Source_Collection()
        {
            if (ContainsInitialCapacity)
            {
                var array = CreatArray(25);
                var collection = CreateCollection(25, array);
                collection.Count.Should().Be(25);
            }
        }
        
        [Fact]
        public void AbstractionCollectionTest_Constructor_Invalid_InitialCapacity_Valid_Source_AbstractionCollection()
        {
            if (ContainsInitialCapacity)
            {
                var array = CreatArray(25);
                var collection1 = CreateCollection(array);
                var collection2 = CreateCollection(25, collection1);
                collection2.Count.Should().Be(25);
            }
        }

        [Fact]
        public void AbstractionCollectionTest_Constructor_Valid_Source_Enumerable()
        {
            var array = CreatArray(25);
            var collection = CreateCollection(Values());
            collection.Count.Should().Be(25);

            IEnumerable<T> Values()
            {
                foreach (var item in array)
                {
                    yield return item;
                }
            }
        }

        [Fact]
        public void AbstractionCollectionTest_Constructor_Valid_Source_Collection()
        {
            var array = CreatArray(25);
            var collection = CreateCollection(array);
            collection.Count.Should().Be(25);
        }
        
        [Fact]
        public void AbstractionCollectionTest_Constructor_Valid_Source_AbstractCollection()
        {
            var array = CreatArray(25);
            var collection1 = CreateCollection(array);
            var collection2 = CreateCollection(collection1);
            collection2.Count.Should().Be(25);
        }
        
        [Fact]
        public void AbstractionCollectionTest_Constructor_Invalid_Source()
        {
            Assert.Throws<ArgumentNullException>(() => CreateCollection(null!));
        }

        #endregion

        #region Clone

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(75)]
        [InlineData(100)]
        public void AbstractionCollectionTest_Clone(int size)
        {
            var collection = CreateCollection(CreatArray(size));
            if (collection is ICloneable cloneable)
            {
                var clone =  (AbstractCollection<T>)cloneable.Clone();
                clone.Count.Should().Be(size);
                clone.Should().BeEquivalentTo(collection);
            }
            
        }

        #endregion
        
        private readonly struct DefaultIEqualityComparer : IEqualityComparer<T>
        {
            private readonly IComparer<T> _comparer;

            public DefaultIEqualityComparer(IComparer<T> comparer)
            {
                _comparer = comparer;
            }

            public bool Equals(T x, T y)
            {
                return _comparer.Compare(x, y) == 0;
            }

            public int GetHashCode(T obj)
            {
                return EqualityComparer<T>.Default.GetHashCode(obj);
            }
        }
    }
}