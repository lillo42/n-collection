using System;
using System.Collections;
using System.Diagnostics;
using NCollection.DebugViews;

namespace NCollection
{
    /// <summary>
    ///  Represents a first-in, first-out (FIFO) collection of <see cref="object"/>  with linked node.
    /// </summary>
    [DebuggerTypeProxy(typeof(ICollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class LinkedQueue : IQueue
    {
        private Node? _root;
        private Node? _last;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedQueue"/>.
        /// </summary>
        public LinkedQueue()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedQueue"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public LinkedQueue(IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            
            foreach (var item in enumerable)
            {
                Enqueue(item);
            }
        }
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual int Count { get; private set; }
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual bool IsSynchronized => false;
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual object SyncRoot => this;
        
        /// <inheritdoc cref="ICollection"/>
        public virtual bool IsEmpty => _root == null;

        /// <inheritdoc cref="ICollection"/>
        public virtual bool IsReadOnly => false;

        /// <inheritdoc cref="IQueue"/>
        public virtual void Enqueue(object? item)
        {
            if (_root == null)
            {
                _last = _root = new Node(item);
            }
            else
            {
                _last!.Next = new Node(item);
                _last = _last!.Next;
            }

            Count++;
        }

        /// <inheritdoc cref="IQueue"/>
        public virtual object? Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        /// <inheritdoc cref="IQueue"/>
        public virtual bool TryPeek(out object? item)
        {
            if (_root == null)
            {
                item = null;
                return false;
            }

            item = _root.Value;
            return true;
        }

        /// <inheritdoc cref="IQueue"/>
        public virtual object? Dequeue()
        {
            if (TryDequeue(out var value))
            {
                return value;
            }

            throw new InvalidOperationException("Empty queue");
        }

        /// <inheritdoc cref="IQueue"/>
        public virtual bool TryDequeue(out object? item)
        {
            if (_root == null)
            {
                item = null;
                return false;
            }

            var dispose = _root;
            item = _root.Value;
            _root = _root.Next;
            dispose.Dispose();
            Count--;
            return true;
        }

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual void Clear()
        {
            var current = _root;
            while (current != null)
            {
                var item = current;
                current = current.Next;
                item.Dispose();
            }

            Count = 0;
        }

        /// <summary>
        /// Returns an <see cref="LinkedQueueEnumerator"/> for the <see cref="LinkedQueue"/>.
        /// </summary>
        /// <returns>An <see cref="LinkedQueueEnumerator"/>  for the <see cref="LinkedQueue"/>.</returns>
        public LinkedQueueEnumerator GetEnumerator()
            => new LinkedQueueEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <inheritdoc cref="System.Collections.ICollection"/>
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

            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index out  of range");
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException("Invalid Off length");
            }

            var source = new object?[Count];

            if (_root != null)
            {
                var current = _root;
                for (var i = 0; current != null; i++)
                {
                    source[i] = current.Value;
                    current = current.Next!;
                } 
            }

            try
            {
                Array.Copy(source, 0, array, index, Count);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException("Invalid array type", nameof(array));
            }
        }

        private class Node : IDisposable
        {
            public Node(object? value)
            {
                Value = value;
            }

            public object? Value { get; set; }
            public Node? Next { get; set; }

            public void Dispose()
            {
                Value = null;
                Next = null;
            }
        }

        /// <inheritdoc cref="IQueue"/>
        public virtual bool Contains(object? item)
        {
            var current = _root;

            while (current != null)
            {
                if (item == null)
                {
                    if (current.Value == null)
                    {
                        return true;
                    }
                }
                else if (current.Value != null && current.Value.Equals(item))
                {
                    return true;
                }
                
                current = current.Next;
            }

            return false;
        }

        /// <summary>
        /// Creates a shallow copy of the <see cref="LinkedQueue"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="LinkedQueue"/>.</returns>
        public LinkedQueue Clone() 
            => new LinkedQueue(this);

        object ICloneable.Clone()
            => Clone();

        /// <summary>
        /// Implementation of <see cref="IEnumerable"/> for <see cref="LinkedQueue"/>.
        /// </summary>
        public struct LinkedQueueEnumerator : IEnumerator
        {
            private readonly LinkedQueue _queue;
            private Node? _current;

            /// <summary>
            /// Initializes a new instance of the <see cref="LinkedQueueEnumerator"/>.
            /// </summary>
            /// <param name="queue">The <see cref="LinkedQueue"/> queue.</param>
            public LinkedQueueEnumerator(LinkedQueue queue)
            {
                _queue = queue;
                _current = _queue._root;
                Current = null;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_current == null)
                {
                    Current = null;
                    return false;
                }

                Current = _current.Value;
                _current = _current.Next;
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _current = _queue._root;
            }

            /// <inheritdoc />
            public object? Current { get; private set; }
        }
    }
}