using System;
using System.Collections;
using System.Diagnostics;
using NCollection.DebugViews;

namespace NCollection
{
    /// <summary>
    /// Represents a last-in-first-out (LIFO) non-generic collection of <see cref="object"/> with linked node.
    /// </summary>
    [DebuggerTypeProxy(typeof(ICollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class LinkedStack : IStack
    {
        private Node? _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedStack"/>.
        /// </summary>
        public LinkedStack()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedStack"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public LinkedStack(IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            
            var item = enumerable.GetEnumerator();
            while (item.MoveNext())
            {
                Push(item.Current);
            }
        }
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public int Count { get; private set; }

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public bool IsSynchronized => false;

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public object SyncRoot => this;

        /// <inheritdoc cref="ICollection"/>
        public bool IsEmpty => _current == null;

        /// <inheritdoc cref="ICollection"/>
        public bool IsReadOnly => false;
        
        /// <inheritdoc cref="IStack"/>
        public object? Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }
        
        /// <inheritdoc cref="IStack"/>
        public bool TryPeek(out object? item)
        {
            if (_current == null)
            {
                item = null;
                return false;
            }

            item = _current.Value;
            return true;
        }

        /// <inheritdoc cref="IStack"/>
        public void Push(object? item)
        {
            _current = new Node(_current, item);
            Count++;
        }
        
        /// <inheritdoc cref="IStack"/>
        public object? Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        /// <inheritdoc cref="IStack"/>
        public bool TryPop(out object? item)
        {
            if (Count == 0 || _current == null)
            {
                item = null;
                return false;
            }

            var dispose = _current;
            item = _current.Value;
            _current = _current.Preview;
            dispose.Dispose();
            Count--;
            return true;
        }

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
            
            var current = _current;
            for (var i = 0; current != null; i++)
            {
                source[i] = current.Value;
                current = current.Preview;
            }

            try
            {
                Array.Copy(source, 0, array, index, Count);
            }
            catch (ArrayTypeMismatchException ex)
            {
                throw new ArgumentException("Invalid array type", nameof(array), ex);
            }
        }
        
        /// <inheritdoc cref="ICollection"/>
        public bool Contains(object? item)
        {
            var node = _current;
            
            while (node != null)
            {
                if (item == null)
                {
                    if (node.Value == null)
                    {
                        return true;
                    }
                }
                else if (node.Value != null && node.Value.Equals(item))
                {
                    return true;
                }
                
                node = node.Preview;
            } 

            return false;
        }
        
        /// <inheritdoc cref="ICollection"/>
        public void Clear()
        {
            var current = _current;
            while (current != null)
            {
                var item = current;
                current = current.Preview;
                item.Dispose();
            }

            Count = 0;
        }

        /// <summary>
        /// Creates a shallow copy of the <see cref="LinkedStack"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="LinkedStack"/>.</returns>
        public LinkedStack Clone()
        {
           var clone =  new LinkedStack();
           
           var array = new object[Count];
           CopyTo(array, 0);

           for (var i = array.Length - 1 ; i >= 0; i--)
           {
               clone.Push(array[i]);
           }

           return clone;
        }

        /// <summary>
        /// Returns an <see cref="LinkedArrayEnumerator"/> for the <see cref="LinkedStack"/>.
        /// </summary>
        /// <returns>An <see cref="LinkedArrayEnumerator"/>  for the <see cref="LinkedStack"/>.</returns>
        public LinkedArrayEnumerator GetEnumerator()
            => new LinkedArrayEnumerator(this);
        
        object ICloneable.Clone()
            => Clone();

        IEnumerator IEnumerable.GetEnumerator() 
            => new LinkedArrayEnumerator(this);

        private sealed class Node : IDisposable
        {
            public Node(Node? preview, object? value)
            {
                Preview = preview;
                Value = value;
            }

            internal object? Value { get; set; }
            internal Node? Preview { get; set; }

            public void Dispose()
            {
                Value = null;
                Preview = null;
            }
        }

        /// <summary>
        /// Implementation of <see cref="IEnumerable"/> for <see cref="LinkedStack"/>.
        /// </summary>
        public struct LinkedArrayEnumerator : IEnumerator
        {
            private readonly LinkedStack _stack;
            private Node? _current;

            /// <summary>
            /// Initializes a new instance of the <see cref="LinkedArrayEnumerator"/>.
            /// </summary>
            /// <param name="stack">The <see cref="LinkedStack"/>.</param>
            public LinkedArrayEnumerator(LinkedStack stack)
            {
                _stack = stack;
                _current = _stack._current;
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
                _current = _current.Preview;
                return true;
            }

            /// <inheritdoc />
            public void Reset() => _current = _stack._current;

            /// <inheritdoc />
            public object? Current { get; private set; }
        }
    }
}