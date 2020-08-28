using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    public class PriorityQueue<T> : AbstractQueue<T>
    {
        /// <summary>
        /// The <see cref="IComparer{T}"/>
        /// </summary>
        public IComparer<T> Comparer { get; }

        private Node[] _elements;
        private int _version = int.MinValue;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        public PriorityQueue()
        {
            Comparer = Comparer<T>.Default;
            _elements = ArrayPool<Node>.Shared.Rent(16);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="initialCapacity"/>.
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
            _elements = ArrayPool<Node>.Shared.Rent(initialCapacity);
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
            _elements = ArrayPool<Node>.Shared.Rent(initialCapacity);
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

                    _elements = ArrayPool<Node>.Shared.Rent(priorityQueue._elements.Length);
                    Array.Copy(priorityQueue._elements, _elements, priorityQueue.Count);
                    break;
                default:
                {
                    Comparer = Comparer<T>.Default;
                    _elements = ArrayPool<Node>.Shared.Rent(16);

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

                _elements = ArrayPool<Node>.Shared.Rent(priorityQueue._elements.Length);
                Array.Copy(priorityQueue._elements, _elements, priorityQueue.Count);
            }
            else
            {
                _elements = ArrayPool<Node>.Shared.Rent(16);

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
                var tmp = ArrayPool<Node>.Shared.Rent(_elements.Length << 1);
                Array.Copy(_elements, tmp, Count);
                ArrayPool<Node>.Shared.Return(_elements, true);
                _elements = tmp;
            }


            SiftUp(Count, new Node(item, Comparer));
            _version++;
            Count++;
            return true;
        }

        private void SiftUp(int position, Node item)
        {
            while (position > 0)
            {
                var parentPosition = (position - 1) >> 1;
                var parent = _elements[parentPosition];

                if (Comparer.Compare(parent.Value, item.Value) >= 0)
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

            item = _elements[0].Value;
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
        private static T Dequeue(Node[] queue, int size, IComparer<T> comparer)
        {
            if (size >= 0)
            {
                var root = queue[0];
                var position = --size;

                var temp = queue[position];
                queue[position] = default!;
                if (position > 0)
                {
                    SiftDown(0, temp, position, queue, comparer);
                }
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