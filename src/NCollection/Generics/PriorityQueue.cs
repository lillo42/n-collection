using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NCollection.Generics.DebugViews;

namespace NCollection.Generics
{
    /// <summary>
    /// An unbounded priority <see cref="IQueue{T}"/> based on a priority heap.
    /// The elements of the priority queue are ordered according to their <see cref="IComparer{T}"/>
    /// provided at queue construction time, depending on which constructor is
    /// used.  A priority queue does not permit null elements.
    /// A priority queue relying on natural ordering also does not permit
    ///  insertion of non-comparable objects.
    /// 
    /// The head of this queue is the least element
    /// with respect to the specified ordering.  If multiple elements are
    /// tied for least value, the head is one of those elements -- ties are
    /// broken arbitrarily.  The queue retrieval operations <see cref="Dequeue"/>,
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
    /// Note that this implementation is not synchronized.
    /// Multiple threads should not access a <see cref="PriorityQueue{T}"/>
    /// instance concurrently if any of the threads modifies the queue.
    /// 
    /// Implementation note: this implementation provides
    /// O(log(n)) time for the enqueuing and dequeuing methods
    /// (<see cref="Enqueue"/> and <see cref="Dequeue"/>);
    /// linear time for the <see cref="Remove"/> and <see cref="Contains"/>
    /// methods; and constant time for the retrieval methods
    /// (<see cref="Peek"/> and <see cref="Count"/>).
    /// </summary>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class PriorityQueue<T> : IQueue<T>, IQueue
    {
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}.Count" />
        public int Count { get; private set; }

        /// <inheritdoc />
        public bool IsSynchronized => false;
        
        /// <inheritdoc />
        public object SyncRoot => this;

        /// <inheritdoc cref="ICollection{T}.IsEmpty" />
        public bool IsEmpty => Count == 0;

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}.IsReadOnly" />
        public bool IsReadOnly => false;

        /// <summary>
        /// The comparator.
        /// </summary>
        public IComparer<T> Comparer { get; }

        private const int DefaultValue = 4;
        private Node[] _queue;
        private int _version = int.MinValue;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue"/>.
        /// </summary>
        public PriorityQueue()
        {
            Comparer = Comparer<T>.Default;
            _queue = ArrayPool<Node>.Shared.Rent(DefaultValue);
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

            Comparer = Comparer<T>.Default;
            _queue = ArrayPool<Node>.Shared.Rent(initialCapacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this priority queue</param>
        /// <param name="initialCapacity">The initial number of elements that the <see cref="ArrayList"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/>is less than zero.</exception>
        public PriorityQueue(int initialCapacity, IComparer<T> comparer)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Need non-negative number");
            }

            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _queue = ArrayPool<Node>.Shared.Rent(initialCapacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public PriorityQueue(IEnumerable<T> enumerable)
        {
            switch (enumerable)
            {
                case null:
                    throw new ArgumentNullException(nameof(enumerable));
                case PriorityQueue<T> priorityQueue:
                    Comparer = priorityQueue.Comparer;
                    Count = priorityQueue.Count;
                    _version = priorityQueue._version;
                
                    _queue = ArrayPool<Node>.Shared.Rent(priorityQueue._queue.Length);
                    Array.Copy(priorityQueue._queue, _queue, Count);
                    break;
                default:
                {
                    Comparer = Comparer<T>.Default;
                    _queue = ArrayPool<Node>.Shared.Rent(DefaultValue);

                    foreach (var item in enumerable)
                    {
                        Enqueue(item);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to copy elements from.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this priority queue</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public PriorityQueue(IEnumerable<T> enumerable, IComparer<T> comparer)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            if (enumerable is PriorityQueue<T> priorityQueue)
            {
                Count = priorityQueue.Count;
                _version = priorityQueue._version;
                
                _queue = ArrayPool<Node>.Shared.Rent(priorityQueue._queue.Length);
                Array.Copy(priorityQueue._queue, _queue, Count);
            }
            else
            {
                _queue = ArrayPool<Node>.Shared.Rent(DefaultValue);

                foreach (var item in enumerable)
                {
                    Enqueue(item);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this priority queue</param>
        public PriorityQueue(IComparer<T> comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _queue = ArrayPool<Node>.Shared.Rent(DefaultValue);
        }

        #endregion

        #region Enqueue

        /// <summary>
        /// Inserts the specified element into this priority queue.
        /// </summary>
        /// <param name="item">The <see cref="object"/> to add to the <see cref="PriorityQueue{T}"/>.</param>
        public void Enqueue(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            if (Count == _queue.Length)
            {
                var temp = ArrayPool<Node>.Shared.Rent(_queue.Length * 2);
                Array.Copy(_queue, temp, Count);
                ArrayPool<Node>.Shared.Return(_queue, true);
                _queue = temp;
            }

            SiftUp(Count, new Node(item, Comparer), _queue, Comparer);
            Count++;
            _version++;
        }
        
        void IQueue.Enqueue(object? item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            Enqueue((T)item!);
        } 


        /// <summary>
        /// Inserts item x at position k, maintaining heap invariant by
        /// promoting x up the tree until it is greater than or equal to
        /// its parent, or is the root.
        /// </summary>
        /// <param name="position">the position to fill</param>
        /// <param name="item">the item to insert</param>
        /// <param name="queue">The array queue</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/>.</param>
        private static void SiftUp(int position, Node item, Node[] queue, IComparer<T> comparer)
        {
            while (position > 0)
            {
                var parent = (position - 1) >> 1;
                var value = queue[parent];

                if (comparer.Compare(item.Value, value.Value) >= 0)
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
        public bool TryPeek([NotNullWhen(true)] out T item)
        {
            if (Count == 0)
            {
                item = default!;
                return false;
            }

            item = _queue[0].Value;
            return true;
        }

        /// <inheritdoc />
        public T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        bool IQueue.TryPeek(out object? item)
        {
            if (!TryPeek(out var value))
            {
                throw new InvalidOperationException("Empty queue");
            }

            item = value;
            return true;
        }

        #endregion

        #region Dequeue

        /// <inheritdoc />
        public bool TryDequeue([MaybeNull] out T item)
        {
            if (IsEmpty)
            {
                item = default;
                return false;
            }
            
            item = RemoveFirst(_queue, Count, Comparer);
            Count--;
            _version++;
            
            return true;
        }

        /// <inheritdoc />
        public bool Remove(T item)
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
                _queue[index] = default!;
            }
            else
            {
                var moved = _queue[Count];
                SiftDown(index, moved, Count, _queue, Comparer);
                if (Comparer.Compare(_queue[index].Value, moved.Value) == 0)
                {
                    SiftUp(index, moved, _queue, Comparer);
                }
            }

            return true;
        }

        [return: MaybeNull]
        private static T RemoveFirst(Node[] queue, int size, IComparer<T> comparer)
        {
            if (size >= 0)
            {
                var item = queue[0];
                var position = --size;
                var temp = queue[position];
                queue[position] = default!;
                if (position > 0)
                {
                    SiftDown(0, temp, position, queue, comparer);
                }

                return item.Value;
            }

            return default;
        }

        private static void SiftDown(int position, Node item, int n, Node[] queue, IComparer<T> comparer)
        {
            var half = n >> 1;
            while (position < half)
            {
                var childPosition = (position << 1) + 1;
                var child = queue[childPosition];
                var right = childPosition + 1;
                if (right < n && comparer.Compare(child.Value, queue[right].Value) > 0)
                {
                    childPosition = right;
                    child = queue[right];
                }

                if (comparer.Compare(item.Value, child.Value) <= 0)
                {
                    break;
                }

                queue[position] = child;
                position = childPosition;
            }

            queue[position] = item;
        }

        /// <inheritdoc />
        public T Dequeue()
        {
            if (!TryDequeue(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item!;
        }

        bool IQueue.TryDequeue(out object? item)
        {
            if (!TryDequeue(out var value))
            {
                throw new InvalidOperationException("Empty queue");
            }

            item = value;
            return value != null;
        }

        #endregion

        #region CopyTo

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            
            if (array.Rank != 1)
            {
                throw new ArgumentException("Rank multi-dimensional not supported", nameof(array));
            }
        
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Need no negative number");
            }
        
            if (arrayIndex > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Index out  of range");
            }
        
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Invalid length");
            }

            using var enumerable = GetEnumerator();
            var counter = arrayIndex;
        
            while (enumerable.MoveNext())
            {
                array[counter++] = enumerable.Current; 
            }
        }


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

            using var enumerable = GetEnumerator();
            var counter = index;

            while (enumerable.MoveNext())
            {
                array.SetValue(enumerable.Current, counter++);
            }
        }

        #endregion

        #region Contains

        bool ICollection.Contains(object? item) => Contains((T) item!);

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        private int IndexOf(T item)
        {
            if (item != null)
            {
                for (var i = 0; i < _queue.Length; i++)
                {
                    if (item.Equals(_queue[i].Value))
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
        public PriorityQueue<T> Clone()
        {
            return new PriorityQueue<T>(this);
        }

        object ICloneable.Clone() 
            => Clone();

        #endregion

        #region IEnumerator

        /// <summary>
        /// Returns an <see cref="PriorityQueueEnumerator"/> for the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <returns>An <see cref="PriorityQueueEnumerator"/>  for the <see cref="PriorityQueue{T}"/>.</returns>
        public PriorityQueueEnumerator GetEnumerator() 
            => new PriorityQueueEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => GetEnumerator();

        /// <summary>
        /// Implementation of <see cref="IEnumerable"/> for <see cref="PriorityQueueEnumerator"/>.
        /// </summary>
        public struct PriorityQueueEnumerator : IEnumerator<T>
        {
            private readonly PriorityQueue<T> _priority;
            private readonly Node[] _queue;
            private readonly int _version;
            private int _size;

            /// <summary>
            /// Initializes a new instance of the <see cref="PriorityQueueEnumerator"/>.
            /// </summary>
            /// <param name="priority">The <see cref="PriorityQueue{T}"/>.</param>
            public PriorityQueueEnumerator(PriorityQueue<T> priority)
            {
                _priority = priority ?? throw new ArgumentNullException(nameof(priority));
                _version = priority._version;
                
                _queue = new Node[_priority.Count + 1];
                _size = priority.Count;
                
                Array.Copy(_priority._queue, _queue, _priority.Count);
                Current = default!;
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
                
                Current = RemoveFirst(_queue, _size, _priority.Comparer)!;
                _size--;
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _size = _priority.Count;
                Array.Copy(_priority._queue, _queue, _queue.Length);
            }

            object? IEnumerator.Current => Current;

            /// <inheritdoc />
            public T Current { get; private set; }

            /// <inheritdoc />
            public void Dispose()
            {
                
            }
        }

        #endregion

        private class Node : IEquatable<Node>
        {
            public Node(T value, IComparer<T> comparer)
            {
                Value = value;
                Comparer = comparer;
            }

            public T Value { get; }

            public IComparer<T> Comparer { get; }


            public bool Equals(Node? other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                
                return Comparer.Compare(Value, other.Value) == 0;
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }
                
                return Equals((Node) obj);
            }

            public override int GetHashCode()
            {
                return EqualityComparer<T>.Default.GetHashCode(Value);
            }

            public static bool operator ==(Node? left, Node? right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Node? left, Node? right)
            {
                return !Equals(left, right);
            }
        }
    }
}

