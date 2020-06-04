using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NCollection.Generics.DebugViews;

namespace NCollection.Generics
{
    /// <summary>
    /// Represents a last-in-first-out (LIFO) generic collection of <typeparamref name="T"/> with linked node.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the stack.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class LinkedStack<T> : IStack<T>, IStack
    {
        private Node? _current;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedStack{T}"/>.
        /// </summary>
        public LinkedStack()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedStack{T}"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public LinkedStack(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            
            
            foreach (var item in enumerable)
            {
                Push(item);
            }
        }
        
        /// <inheritdoc cref="IStack{T}"/>
        public int Count { get; private set; }

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public bool IsSynchronized => false;

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public object SyncRoot => this;

        /// <inheritdoc cref="IStack"/>
        public bool IsEmpty => _current == null;

        /// <inheritdoc cref="IStack{T}"/>
        public bool IsReadOnly => false;

        /// <inheritdoc cref="IStack{T}"/>
        [return: MaybeNull]
        public T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public bool TryPeek([MaybeNullWhen(false)]out T item)
        {
            if (_current == null)
            {
                item = default!;
                return false;
            }

            item = _current.Value;
            return true;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public void Push(T item)
        {
            _current = new Node(_current, item);
            Count++;
        }

        /// <inheritdoc cref="IStack{T}"/>
        [return: MaybeNull]
        public T Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public void Clear()
        {
            while (_current != null)
            {
                var item = _current;
                _current = _current.Preview;
                item.Dispose();
            }

            Count = 0;
        }

        

        /// <inheritdoc cref="IStack{T}"/>
        public bool TryPop([MaybeNullWhen(false)]out T item)
        {
            if (Count == 0 || _current == null)
            {
                item = default!;
                return false;
            }

            var dispose = _current;
            item = dispose.Value;
            _current = _current.Preview;
            Count--;
            dispose.Dispose();
            return true;
        }
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public bool Contains(T item)
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
                else if (node.Value != null && EqualityComparer<T>.Default.Equals(node.Value, item))
                {
                    return true;
                }
                
                node = node.Preview;
            } 

            return false;
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

            
            var source = new T[Count];
            
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
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Index out  of range");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Invalid Off length");
            }
            
            var current = _current;
            for (var i = 0; current != null; i++)
            {
                array[i + arrayIndex] = current.Value;
                current = current.Preview;
            }
        }

        /// <summary>
        /// Returns an <see cref="LinkedStackEnumerator"/> for the <see cref="LinkedStack{T}"/>.
        /// </summary>
        /// <returns>An <see cref="LinkedStackEnumerator"/>  for the <see cref="LinkedStack{T}"/>.</returns>
        public LinkedStackEnumerator GetEnumerator()
            => new LinkedStackEnumerator(this);

        /// <summary>
        /// Creates a shallow copy of the <see cref="LinkedStack{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="LinkedStack{T}"/>.</returns>
        public LinkedStack<T> Clone() 
        {
            var clone =  new LinkedStack<T>();
           
            var array = new T[Count];
            CopyTo(array, 0);

            for (var i = array.Length - 1 ; i >= 0; i--)
            {
                clone.Push(array[i]);
            }

            return clone;
        }

        object ICloneable.Clone()
            => Clone();
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
        
        #region Stack
        void IStack.Push(object? item)
            => Push((T) item!);

        bool IStack.TryPop(out object? item)
        {
            if (TryPop(out var result))
            {
                item = result;
                return true;
            }

            item = null;
            return false;
        }
        
        bool IStack.TryPeek(out object? item)
        {
            if (TryPeek(out var result))
            {
                item = result;
                return true;
            }

            item = null;
            return false;
        }

        bool ICollection.Contains(object? item) => Contains((T) item);
        #endregion

        private sealed class Node : IDisposable
        {
            public Node(Node? preview, T value)
            {
                Preview = preview;
                Value = value;
            }

            internal T Value { get; private set; }
            internal Node? Preview { get; private set; }

            public void Dispose()
            {
                Value = default!;
                Preview = null;
            }
        }

        /// <summary>
        /// Implementation of <see cref="IEnumerable{T}"/> for <see cref="LinkedStack{T}"/>.
        /// </summary>
        public struct LinkedStackEnumerator : IEnumerator<T>
        {
            private readonly LinkedStack<T> _stack;
            private Node? _current;

            /// <summary>
            /// Initializes a new instance of the <see cref="LinkedStackEnumerator"/>.
            /// </summary>
            /// <param name="stack">The <see cref="LinkedStack{T}"/>.</param>
            public LinkedStackEnumerator(LinkedStack<T> stack)
            {
                _stack = stack;
                _current = _stack._current;
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
                _current = _current.Preview;
                return true;
            }

            /// <inheritdoc />
            public void Reset() 
                => _current = _stack._current;
            
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