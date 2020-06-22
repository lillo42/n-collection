using System;
using System.Buffers;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    /// 
    /// </summary>
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
                _queue = ArrayPool<object>.Shared.Rent(priorityQueue._queue.Length);
                Array.Copy(priorityQueue._queue, _queue, _queue.Length);
                Comparer = priorityQueue.Comparer;
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
                _queue = ArrayPool<object>.Shared.Rent(priorityQueue._queue.Length);
                Array.Copy(priorityQueue._queue, _queue, _queue.Length);
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
            
            SiftUp(Count, item);
            Count++;
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
        private void SiftUp(int position, object item)
        {
            while (position > 0)
            {
                var parent = (position - 1) >> 1;
                var value = _queue[parent];
                
                if (Comparer.Compare(item, value) >= 0)
                {
                    break;
                }

                _queue[position] = value;
                position = parent;
            }

            _queue[position] = item;
        }

        #endregion

        #region Peek
        
        /// <inheritdoc />
        public bool TryPeek([NotNullWhen(true)]out object? item)
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
        public bool TryDequeue([MaybeNullWhen(true)]out object? item)
        {
            item = _queue[0];
            if (item != null)
            {
                var position = --Count;
                var temp = _queue[position];
                _queue[position] = null!;
                if (position > 0)
                {
                    SiftDown(0, temp, position);
                }
            }

            return item != null;
        }

        private void SiftDown(int position, object item, int n)
        {
            var half = n >> 1;
            while (position < half)
            {
                var childPosition = (position << 1) + 1;
                var child = _queue[childPosition];
                var right = childPosition + 1;
                if (right < n && Comparer.Compare(child, _queue[right]) > 0)
                {
                    childPosition = right;
                    child = _queue[right];
                }

                if (Comparer.Compare(item, child) <= 0)
                {
                    break;
                }

                _queue[position] = child;
                position = childPosition;
            }

            _queue[position] = item;
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
            
            Array.Copy(_queue, 0, array, index, Count);
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
            var priority = new PriorityQueue(Comparer);
            
            Array.Copy(_queue, priority._queue, _queue.Length);
            
            return priority;
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
            => new PriorityQueueEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public struct PriorityQueueEnumerator : IEnumerator
        {
            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public object Current => throw new NotImplementedException();
        }

        #endregion
    }
}

/*
 * /**
 * An unbounded priority {@linkplain Queue queue} based on a priority heap.
 * The elements of the priority queue are ordered according to their
 * {@linkplain Comparable natural ordering}, or by a {@link Comparator}
 * provided at queue construction time, depending on which constructor is
 * used.  A priority queue does not permit {@code null} elements.
 * A priority queue relying on natural ordering also does not permit
 * insertion of non-comparable objects (doing so may result in
 * {@code ClassCastException}).
 *
 * <p>The <em>head</em> of this queue is the <em>least</em> element
 * with respect to the specified ordering.  If multiple elements are
 * tied for least value, the head is one of those elements -- ties are
 * broken arbitrarily.  The queue retrieval operations {@code poll},
 * {@code remove}, {@code peek}, and {@code element} access the
 * element at the head of the queue.
 *
 * <p>A priority queue is unbounded, but has an internal
 * <i>capacity</i> governing the size of an array used to store the
 * elements on the queue.  It is always at least as large as the queue
 * size.  As elements are added to a priority queue, its capacity
 * grows automatically.  The details of the growth policy are not
 * specified.
 *
 * <p>This class and its iterator implement all of the
 * <em>optional</em> methods of the {@link Collection} and {@link
 * Iterator} interfaces.  The Iterator provided in method {@link
 * #iterator()} and the Spliterator provided in method {@link #spliterator()}
 * are <em>not</em> guaranteed to traverse the elements of
 * the priority queue in any particular order. If you need ordered
 * traversal, consider using {@code Arrays.sort(pq.toArray())}.
 *
 * <p><strong>Note that this implementation is not synchronized.</strong>
 * Multiple threads should not access a {@code PriorityQueue}
 * instance concurrently if any of the threads modifies the queue.
 * Instead, use the thread-safe {@link
 * java.util.concurrent.PriorityBlockingQueue} class.
 *
 * <p>Implementation note: this implementation provides
 * O(log(n)) time for the enqueuing and dequeuing methods
 * ({@code offer}, {@code poll}, {@code remove()} and {@code add});
 * linear time for the {@code remove(Object)} and {@code contains(Object)}
 * methods; and constant time for the retrieval methods
 * ({@code peek}, {@code element}, and {@code size}).
 *
 * <p>This class is a member of the
 * <a href="{@docRoot}/java.base/java/util/package-summary.html#CollectionsFramework">
 * Java Collections Framework</a>.
 *
 * @since 1.5
 * @author Josh Bloch, Doug Lea
 * @param <E> the type of elements held in this queue
 */