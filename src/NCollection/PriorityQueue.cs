using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    /// An unbounded priority <see cref="IQueue{T}"/> based on a priority heap.
    /// The elements of the priority queue are ordered according to their <see cref="IComparer{T}"/>
    /// provided at queue construction time, depending on which constructor is
    /// used.  A priority queue does not permit null elements.
    /// A priority queue relying on natural ordering also does not permit
    /// insertion of non-comparable objects.
    /// 
    /// The head of this queue is the least element
    /// with respect to the specified ordering.  If multiple elements are
    /// tied for least value, the head is one of those elements -- ties are
    /// broken arbitrarily.  The queue retrieval operations <see cref="AbstractQueue{T}.Dequeue"/>,
    /// <see cref="Remove"/> and <see cref="AbstractQueue{T}.Peek"/> access the
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
    /// (<see cref="AbstractQueue{T}.Enqueue"/> and <see cref="AbstractQueue{T}.Dequeue"/>);
    /// linear time for the <see cref="Remove"/> and <see cref="AbstractCollection{T}.Contains"/>
    /// methods; and constant time for the retrieval methods
    /// (<see cref="AbstractQueue{T}.Peek"/> and <see cref="AbstractCollection{T}.Count"/>).
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class PriorityQueue<T> : AbstractQueue<T>, ITree<T>, ICloneable
    {
        /// <summary>
        /// The <see cref="IComparer{T}"/>
        /// </summary>
        public IComparer<T> Comparer { get; }

        private Node?[] _elements;
        private int _version = int.MinValue;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        public PriorityQueue()
        {
            Comparer = Comparer<T>.Default;
            _elements = ArrayPool<Node?>.Shared.Rent(16);
        }

        /// <summary>
        /// Initializes a new instance of the <paramref name="initialCapacity"/>.
        /// </summary>
        /// <param name="initialCapacity">The initial number of elements that the <see cref="PriorityQueue{T}"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/>is less than zero.</exception>
        public PriorityQueue(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Need non-negative number");
            }

            Comparer = Comparer<T>.Default;
            _elements = ArrayPool<Node?>.Shared.Rent(initialCapacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this priority queue</param>
        /// <param name="initialCapacity">The initial number of elements that the <see cref="PriorityQueue{T}"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/>is less than zero.</exception>
        /// /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
        public PriorityQueue(int initialCapacity, IComparer<T> comparer)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Need non-negative number");
            }

            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _elements = ArrayPool<Node?>.Shared.Rent(initialCapacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        public PriorityQueue(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case PriorityQueue<T> priorityQueue:
                    Comparer = priorityQueue.Comparer;
                    // ReSharper disable once VirtualMemberCallInConstructor
                    Count = priorityQueue.Count;

                    _elements = ArrayPool<Node?>.Shared.Rent(priorityQueue._elements.Length);
                    Array.Copy(priorityQueue._elements, _elements, priorityQueue.Count);
                    break;
                default:
                {
                    Comparer = Comparer<T>.Default;
                    _elements = ArrayPool<Node?>.Shared.Rent(16);

                    foreach (var item in source)
                    {
                        // ReSharper disable once VirtualMemberCallInConstructor
                        Enqueue(item);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to copy elements from.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this priority queue</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="comparer"/> is <see langword="null"/></exception>
        public PriorityQueue(IEnumerable<T> source, IComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            if (source is PriorityQueue<T> priorityQueue)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = priorityQueue.Count;
                _version = priorityQueue._version;

                _elements = ArrayPool<Node?>.Shared.Rent(priorityQueue._elements.Length);
                Array.Copy(priorityQueue._elements, _elements, priorityQueue.Count);
            }
            else
            {
                _elements = ArrayPool<Node?>.Shared.Rent(16);

                foreach (var item in source)
                {
                    // ReSharper disable once VirtualMemberCallInConstructor
                    Enqueue(item);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this priority queue</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/></exception>
        public PriorityQueue(IComparer<T> comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _elements = ArrayPool<Node>.Shared.Rent(16);
        }

        #endregion

        /// <inheritdoc />
        public override IEnumerator<T> GetEnumerator()
        {
            return new PriorityQueueEnumerator(this);
        }

        /// <inheritdoc />
        public override bool TryEnqueue(T item)
        {
            if (Count == _elements.Length)
            {
                var tmp = ArrayPool<Node?>.Shared.Rent(_elements.Length << 1);
                Array.Copy(_elements, tmp, Count);
                ArrayPool<Node?>.Shared.Return(_elements, true);
                _elements = tmp;
            }


            SiftUp(Count, new Node(item));
            _version++;
            Count++;
            return true;
        }

        private void SiftUp(int position, Node item)
        {
            while (position > 0)
            {
                var parentPosition = (int)((uint)(position - 1) >> 1);
                var parent = _elements[parentPosition]!;

                if (Comparer.Compare(item.Value, parent.Value) >= 0)
                {
                    break;
                }

                _elements[position] = parent;
                position = parentPosition;
            }

            _elements[position] = item;
        }

        /// <inheritdoc />
        public override bool TryPeek([MaybeNull] out T item)
        {
            if (Count == 0)
            {
                item = default;
                return false;
            }

            item = _elements[0]!.Value;
            return true;
        }

        /// <inheritdoc />
        public override bool TryDequeue([MaybeNull] out T item)
        {
            if (IsEmpty)
            {
                item = default!;
                return false;
            }

            item = Dequeue(_elements, Count, Comparer);
            Count--;
            _version++;
            return true;
        }

        [return: MaybeNull]
        private static T Dequeue(Node?[] queue, int size, IComparer<T> comparer)
        {
            if (size >= 0)
            {
                var root = queue[0];
                var position = --size;

                var temp = queue[position]!;
                queue[position] = default!;
                if (position > 0)
                {
                    SiftDown(0, temp, position, queue, comparer);
                }

                return root!.Value;
            }

            return default;
        }

        private static void SiftDown(int position, Node item, int n, Node?[] queue, IComparer<T> comparer)
        {
            var half = n >> 1;
            while (position < half)
            {
                var childPosition = (position << 1) + 1;
                var child = queue[childPosition];
                var right = childPosition + 1;
                if (right < n && comparer.Compare(child!.Value, queue[right]!.Value) > 0)
                {
                    childPosition = right;
                    child = queue[right];
                }

                if (comparer.Compare(item.Value, child!.Value) <= 0)
                {
                    break;
                }

                queue[position] = child;
                position = childPosition;
            }

            queue[position] = item;
        }
        
        /// <inheritdoc />
        public override bool Remove(T item)
        {
            var index = IndexOf(item, EqualityComparer<T>.Default);

            if (index == -1)
            {
                return false;
            }

            Count--;
            _version++;
            if (Count == index)
            { 
                _elements[index] = null!;
            }
            else
            {
                var moved = _elements[Count]!;
                _elements[Count] = null;
                SiftDown(index, moved, Count, _elements, Comparer);
                if (_elements[index] == moved)
                {
                    SiftUp(index, moved);
                }
            }

            return true;
        }

        private int IndexOf(T item, IEqualityComparer<T> comparer)
        {
            for(var i = 0; i < _elements.Length; i++)
            {
                var node = _elements[i];
                if (node == null)
                {
                    break;
                }
                
                if (comparer.Equals(item, node.Value))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Creates a shallow copy of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="PriorityQueue{T}"/>.</returns>
        public PriorityQueue<T> Clone() => new PriorityQueue<T>(this);
        object ICloneable.Clone() => Clone();
        
        private class Node
        {
            public Node(T value)
            {
                Value = value;
            }

            public T Value { get; }
        }

        private struct PriorityQueueEnumerator : IEnumerator<T>
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

                Array.Copy(_priority._elements, _queue, _priority.Count);
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

                Current = Dequeue(_queue, _size, _priority.Comparer)!;
                _size--;
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _size = _priority.Count;
                Array.Copy(_priority._elements, _queue, _queue.Length);
            }

            object? IEnumerator.Current => Current;

            /// <inheritdoc />
            public T Current { get; private set; }

            /// <inheritdoc />
            public void Dispose()
            {
            }
        }
    }
}