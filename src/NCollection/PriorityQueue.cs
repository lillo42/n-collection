using System;
using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NCollection.DebugViews;

namespace NCollection
{
    /// <summary>
    /// An unbounded priority <see cref="IQueue"/> based on a priority heap.
    /// The elements of the priority queue are ordered according to their <see cref="IComparer"/>
    /// provided at queue construction time, depending on which constructor is
    /// used.  A priority queue does not permit null elements.
    /// A priority queue relying on natural ordering also does not permit
    ///  insertion of non-comparable objects.
    /// 
    /// The head of this queue is the least element
    /// with respect to the specified ordering.  If multiple elements are
    /// tied for least value, the head is one of those elements -- ties are
    /// broken arbitrarily. The queue retrieval operations <see cref="Dequeue"/>,
    /// <see cref="Remove"/> and <see cref="Peek"/> access the
    /// element at the head of the queue.
    /// 
    /// A priority queue is unbounded, but has an internal
    /// capacity governing the size of an array used to store the
    /// elements on the queue.  It is always at least as large as the queue
    /// size.  As elements are added to a priority queue, its capacity
    /// grows automatically.  The details of the growth policy are not
    /// specified.
    /// 
    /// Note that this implementation is not synchronized 
    /// Multiple threads should not access a <see cref="PriorityQueue"/>
    /// instance concurrently if any of the threads modifies the queue.
    /// 
    /// Implementation note: this implementation provides
    /// O(log(n)) time for the enqueuing and dequeuing methods
    /// (<see cref="Enqueue"/> and <see cref="Dequeue"/>);
    /// linear time for the <see cref="Remove"/> and <see cref="Contains"/>
    /// methods; and constant time for the retrieval methods
    /// (<see cref="Peek"/> and <see cref="Count"/>).
    /// </summary>
    [DebuggerTypeProxy(typeof(ICollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    public class PriorityQueue : IQueue
    {
        /// <inheritdoc />
        public int Count { get; private set; }

        /// <inheritdoc />
        public bool IsSynchronized => false;

        /// <inheritdoc />
        public object SyncRoot => this;

        /// <inheritdoc />
        public bool IsEmpty => Count == 0;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <summary>
        /// The comparator.
        /// </summary>
        public IComparer Comparer { get; }

        private const int DefaultValue = 4;
        private object[] _queue;
        private int _version = int.MinValue;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue"/>.
        /// </summary>
        public PriorityQueue()
        {
            Comparer = System.Collections.Comparer.Default;
            _queue = ArrayPool<object>.Shared.Rent(DefaultValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue"/>.
        /// </summary>
        /// <param name="initialCapacity">The initial number of elements that the <see cref="ArrayList"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/>is less than zero.</exception>
        public PriorityQueue(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Need non-negative number");
            }

            Comparer = System.Collections.Comparer.Default;
            _queue = ArrayPool<object>.Shared.Rent(initialCapacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer"/> that will be used to order this priority queue</param>
        /// <param name="initialCapacity">The initial number of elements that the <see cref="ArrayList"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/>is less than zero.</exception>
        public PriorityQueue(int initialCapacity, IComparer comparer)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Need non-negative number");
            }

            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _queue = ArrayPool<object>.Shared.Rent(initialCapacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public PriorityQueue(IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (enumerable is PriorityQueue priorityQueue)
            {
                Comparer = priorityQueue.Comparer;
                Count = priorityQueue.Count;
                _version = priorityQueue._version;
                
                _queue = ArrayPool<object>.Shared.Rent(priorityQueue._queue.Length);
                Array.Copy(priorityQueue._queue, _queue, Count);
            }
            else
            {
                Comparer = System.Collections.Comparer.Default;
                _queue = ArrayPool<object>.Shared.Rent(DefaultValue);

                foreach (var item in enumerable)
                {
                    Enqueue(item);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <param name="comparer">The <see cref="IComparer"/> that will be used to order this priority queue</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public PriorityQueue(IEnumerable enumerable, IComparer comparer)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            if (enumerable is PriorityQueue priorityQueue)
            {
                Count = priorityQueue.Count;
                _version = priorityQueue._version;
                
                _queue = ArrayPool<object>.Shared.Rent(priorityQueue._queue.Length);
                Array.Copy(priorityQueue._queue, _queue, Count);
            }
            else
            {
                _queue = ArrayPool<object>.Shared.Rent(DefaultValue);

                foreach (var item in enumerable)
                {
                    Enqueue(item);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer"/> that will be used to order this priority queue</param>
        public PriorityQueue(IComparer comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _queue = ArrayPool<object>.Shared.Rent(DefaultValue);
        }

        #endregion

        #region Enqueue

        /// <summary>
        /// Inserts the specified element into this priority queue.
        /// </summary>
        /// <param name="item">The <see cref="object"/> to add to the <see cref="PriorityQueue"/>.</param>
        public void Enqueue(object item)
        {
            if (Count == _queue.Length)
            {
                var temp = ArrayPool<object>.Shared.Rent(_queue.Length * 2);
                Array.Copy(_queue, temp, Count);
                ArrayPool<object?>.Shared.Return(_queue, true);
                _queue = temp;
            }

            SiftUp(Count, item, _queue, Comparer);
            Count++;
            _version++;
        }

        void IQueue.Enqueue(object? item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Enqueue(item);
        }


        /// <summary>
        /// Inserts item x at position k, maintaining heap invariant by
        /// promoting x up the tree until it is greater than or equal to
        /// its parent, or is the root.
        /// </summary>
        /// <param name="position">the position to fill</param>
        /// <param name="item">the item to insert</param>
        /// <param name="queue">The array queue</param>
        /// <param name="comparer">The <see cref="IComparer"/>.</param>
        private static void SiftUp(int position, object item, object[] queue, IComparer comparer)
        {
            while (position > 0)
            {
                var parent = (position - 1) >> 1;
                var value = queue[parent];

                if (comparer.Compare(item, value) >= 0)
                {
                    break;
                }

                queue[position] = value;
                position = parent;
            }

            queue[position] = item;
        }

        #endregion

        #region Peek

        /// <inheritdoc />
        public bool TryPeek([NotNullWhen(true)] out object? item)
        {
            if (Count == 0)
            {
                item = null;
                return false;
            }

            item = _queue[0];
            return true;
        }

        /// <inheritdoc />
        public object Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        #endregion

        #region Dequeue

        /// <inheritdoc />
        public bool TryDequeue([MaybeNullWhen(true)] out object? item)
        {
            item = RemoveFirst(_queue, Count, Comparer);
            
            if (item != null)
            {
                Count--;
                _version++;
            }

            return item != null;
        }

        /// <inheritdoc />
        public bool Remove(object? item)
        {
            var index = IndexOf(item);

            if (index == -1)
            {
                return false;
            }

            Count--;
            _version++;
            if (Count == index)
            {
                _queue[index] = null!;
            }
            else
            {
                var moved = _queue[Count];
                SiftDown(index, moved!, Count, _queue, Comparer);
                if (_queue[index] == moved)
                {
                    SiftUp(index, moved, _queue, Comparer);
                }
            }

            return true;
        }

        private static object? RemoveFirst(object[] queue, int size, IComparer comparer)
        {
            var item = queue[0];
            if (item != null)
            {
                var position = --size;
                var temp = queue[position];
                queue[position] = null!;
                if (position > 0)
                {
                    SiftDown(0, temp, position, queue, comparer);
                }
            }

            return item;
        }

        private static void SiftDown(int position, object item, int n, object[] queue, IComparer comparer)
        {
            var half = n >> 1;
            while (position < half)
            {
                var childPosition = (position << 1) + 1;
                var child = queue[childPosition];
                var right = childPosition + 1;
                if (right < n && comparer.Compare(child, queue[right]) > 0)
                {
                    childPosition = right;
                    child = queue[right];
                }

                if (comparer.Compare(item, child) <= 0)
                {
                    break;
                }

                queue[position] = child;
                position = childPosition;
            }

            queue[position] = item;
        }

        /// <inheritdoc />
        public object? Dequeue()
        {
            if (!TryDequeue(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }
        #endregion
        
        
        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            
            if (array.Rank != 1)
            {
                throw new ArgumentException("Rank multi-dimensional not supported", nameof(array));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Need no negative number");
            }

            if (index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index out  of range");
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException("Invalid length");
            }

            var enumerable = GetEnumerator();
            var counter = index;

            while (enumerable.MoveNext())
            {
                array.SetValue(enumerable.Current, counter++);
            }
        }

        #region Contains

        /// <inheritdoc />
        public bool Contains(object? item)
        {
            return IndexOf(item) >= 0;
        }

        private int IndexOf(object? item)
        {
            if (item != null)
            {
                for (var i = 0; i < _queue.Length; i++)
                {
                    if (item.Equals(_queue[i]))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
        #endregion
        
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="PriorityQueue"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="PriorityQueue"/>.</returns>
        public PriorityQueue Clone()
        {
            return new PriorityQueue(this);
        }

        object ICloneable.Clone() 
            => Clone();

        #endregion

        #region IEnumerator
        
        /// <summary>
        /// Returns an <see cref="PriorityQueueEnumerator"/> for the <see cref="PriorityQueue"/>.
        /// </summary>
        /// <returns>An <see cref="PriorityQueueEnumerator"/>  for the <see cref="PriorityQueue"/>.</returns>
        public PriorityQueueEnumerator GetEnumerator() 
            => new PriorityQueueEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        /// <summary>
        /// Implementation of <see cref="IEnumerable"/> for <see cref="PriorityQueueEnumerator"/>.
        /// </summary>
        public struct PriorityQueueEnumerator : IEnumerator
        {
            private readonly PriorityQueue _priority;
            private readonly object[] _queue;
            private readonly int _version;
            private int _size;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="PriorityQueueEnumerator"/>.
            /// </summary>
            /// <param name="priority">The <see cref="PriorityQueue"/>.</param>
            public PriorityQueueEnumerator(PriorityQueue priority)
            {
                _priority = priority ?? throw new ArgumentNullException(nameof(priority));
                _version = priority._version;
                
                _queue = new object[_priority.Count + 1];
                _size = priority.Count;
                
                Array.Copy(_priority._queue, _queue, _priority.Count);
                Current = null;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_version != _priority._version)
                {
                    throw new InvalidOperationException("Priority queue was changed");
                }

                if (_size == 0)
                {
                    return false;
                }
                
                Current = RemoveFirst(_queue, _size, _priority.Comparer);
                _size--;
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _size = _priority.Count;
                Array.Copy(_priority._queue, _queue, _queue.Length);
            }

            /// <inheritdoc />
            public object? Current { get; private set; }
        }

        #endregion
    }
}

