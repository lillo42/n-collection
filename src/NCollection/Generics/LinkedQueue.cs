using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NCollection.Generics.DebugViews;

namespace NCollection.Generics
{
    /// <summary>
    /// Represents a first-in, first-out (FIFO) collection of <typeparamref name="T"/> with linked node.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class LinkedQueue<T> : IQueue<T>
    {
        private Node? _root;
        private Node? _last;
        
        /// <inheritdoc cref="IQueue{T}"/>
        public virtual int Count { get; private set; }
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual bool IsSynchronized => false;
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual object SyncRoot => this;

        /// <inheritdoc cref="ICollection"/>
        public virtual bool IsEmpty => _root == null;

        /// <inheritdoc cref="ICollection"/>
        public virtual bool IsReadOnly => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedQueue{T}"/>.
        /// </summary>
        public LinkedQueue()
        {
            
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedQueue{T}"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public LinkedQueue(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            
            foreach (var value in enumerable)
            {
                Enqueue(value);
            }
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public virtual void Enqueue(T item)
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

        /// <inheritdoc cref="IQueue{T}"/>
        public virtual T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }
        
        /// <inheritdoc cref="IQueue{T}"/>
        public virtual bool TryPeek(out T item)
        {
            if (_root == null)
            {
                item = default!;
                return false;
            }

            item = _root.Value;
            return true;
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public virtual T Dequeue()
        {
            if (TryDequeue(out var value))
            {
                return value;
            }

            throw new InvalidOperationException("Empty queue");
        }
        
        /// <inheritdoc cref="IQueue{T}"/>
        public virtual bool TryDequeue(out T item)
        {
            if (_root == null)
            {
                item = default!;
                return false;
            }

            var dispose = _root;
            item = _root.Value;
            _root = _root.Next;
            dispose.Dispose();
            Count--;
            return true;
        }
        
        /// <inheritdoc cref="IQueue{T}"/>
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
        /// Returns an <see cref="LinkedQueueEnumerator"/> for the <see cref="LinkedQueue{T}"/>.
        /// </summary>
        /// <returns>An <see cref="LinkedQueueEnumerator"/>  for the <see cref="LinkedQueue{T}"/>.</returns>
        public LinkedQueueEnumerator GetEnumerator()
            => new LinkedQueueEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
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

            var source = new T[Count];

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

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
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

            if (arrayIndex < 0 || arrayIndex > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Index out  of range");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Invalid Off length");
            }
            
            if (_root != null)
            {
                var current = _root;
                for (var i = 0; current != null; i++)
                {
                    array[i + arrayIndex] = current.Value;
                    current = current.Next!;
                } 
            }
        }
        
        private class Node : IDisposable
        {
            public Node(T value)
            {
                Value = value;
            }

            public T Value { get; private set; }
            public Node? Next { get; set; }

            public void Dispose()
            {
                Value = default!;
                Next = null;
            }
        }

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public bool Contains(T item)
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
                else if (current.Value != null && EqualityComparer<T>.Default.Equals(current.Value, item))
                {
                    return true;
                }
                
                current = current.Next;
            }

            return false;
        }

        /// <summary>
        /// Creates a shallow copy of the <see cref="LinkedQueue{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="LinkedQueue{T}"/>.</returns>
        public LinkedQueue<T> Clone() 
            => new LinkedQueue<T>(this);

        object ICloneable.Clone()
            => Clone();

        /// <summary>
        /// Implementation of <see cref="IEnumerable{T}"/> for <see cref="LinkedQueue{T}"/>.
        /// </summary>
        public struct LinkedQueueEnumerator : IEnumerator<T>
        {
            private readonly LinkedQueue<T> _queue;
            private Node? _current;

            /// <summary>
            /// Initializes a new instance of the <see cref="LinkedQueueEnumerator"/>.
            /// </summary>
            /// <param name="queue">The <see cref="LinkedQueue{T}"/>.</param>
            public LinkedQueueEnumerator(LinkedQueue<T> queue)
            {
                _queue = queue;
                _current = _queue._root;
                Current = default!;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_current == null)
                {
                    Current = default!;
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

            object? IEnumerator.Current => Current;

            /// <inheritdoc />
            public T Current { get; private set; }
            
            /// <inheritdoc />
            public void Dispose() { }
        }
    }
}