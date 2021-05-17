using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCollection
{
    /// <summary>
    /// The implementation of <see cref="IStack{T}"/> using array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ArrayStack<T> : AbstractStack<T>, ICloneable
    {
        private T[] _elements;
        
        /// <summary>
        /// The <see cref="IArrayProvider{T}"/> instance.
        /// </summary>
        public IArrayProvider<T> ArrayProvider { get; }


        /// <summary>
        /// Initialize <see cref="ArrayStack{T}"/>.
        /// </summary>
        public ArrayStack()
            : this(ArrayProvider<T>.Instance)
        {
            
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayStack{T}"/>.
        /// </summary>
        /// <param name="arrayProvider">The instance <see cref="IArrayProvider{T}"/>.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="arrayProvider"/> is null.</exception>
        public ArrayStack(IArrayProvider<T> arrayProvider)
        {
            ArrayProvider = arrayProvider ?? throw new ArgumentNullException(nameof(arrayProvider));
            _elements = ArrayProvider.GetOrCreate(4);
        }

        /// <summary>
        /// Initialize <see cref="ArrayQueue{T}"/>.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the array.</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0.</exception>
        public ArrayStack(int initialCapacity)
            : this(initialCapacity, ArrayProvider<T>.Instance)
        {
            
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayQueue{T}"/>.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the array.</param>
        /// <param name="arrayProvider">The instance <see cref="IArrayProvider{T}"/>.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="arrayProvider"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0.</exception>
        public ArrayStack(int initialCapacity, IArrayProvider<T> arrayProvider)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The value should be greater or equal than 1");
            }

            ArrayProvider = arrayProvider ?? throw new ArgumentNullException(nameof(arrayProvider));
            _elements = ArrayProvider.GetOrCreate(initialCapacity);
        }

        
        /// <summary>
        /// Initialize <see cref="ArrayStack{T}"/> copying the element in <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">The elements to be copy.</param>
        /// <exception cref="ArgumentNullException">if the  <paramref name="source"/> is <see langword="null"/>.</exception>
        public ArrayStack(IEnumerable<T> source)
            : this(source, ArrayProvider<T>.Instance)
        {
            
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayStack{T}"/> copying the element in <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">The elements to be copy.</param>
        /// <param name="arrayProvider">The instance <see cref="IArrayProvider{T}"/>.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> or <paramref name="arrayProvider"/> is null.</exception>
        public ArrayStack(IEnumerable<T> source, IArrayProvider<T> arrayProvider)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            ArrayProvider = arrayProvider ?? throw new ArgumentNullException(nameof(arrayProvider));
            
            if (source is ArrayStack<T> stack)
            {
                _elements = ArrayProvider.GetOrCreate(stack._elements.Length);
                Array.Copy(stack._elements, _elements, stack.Count);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = stack.Count;
            }
            else if (source is System.Collections.Generic.ICollection<T> collection)
            {
                _elements = ArrayProvider.GetOrCreate(collection.Count);
                collection.CopyTo(_elements, 0);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = collection.Count;
            }
            else
            {
                _elements = ArrayProvider.GetOrCreate(4);
                foreach (var item in source)
                {
                    // ReSharper disable once VirtualMemberCallInConstructor
                    TryPush(item);
                }
            }
        }

        /// <summary>
        /// Initialize <see cref="ArrayStack{T}"/> copying the element in <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">The elements to be copy.</param>
        /// <param name="initialCapacity">The initial capacity of the array.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0.</exception>
        public ArrayStack(int initialCapacity, IEnumerable<T> source)
            : this(initialCapacity, source, ArrayProvider<T>.Instance)
        {
            
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayStack{T}"/> copying the element in <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">The elements to be copy.</param>
        /// <param name="initialCapacity">The initial capacity of the array.</param>
        /// <param name="arrayProvider">The instance <see cref="IArrayProvider{T}"/>.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> or <paramref name="arrayProvider"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0.</exception>
        public ArrayStack(int initialCapacity, IEnumerable<T> source, IArrayProvider<T> arrayProvider)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The value should be greater or equal than 1");
            }
            
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            ArrayProvider = arrayProvider ?? throw new ArgumentNullException(nameof(arrayProvider));

            if (source is ArrayStack<T> stack)
            {
                _elements = ArrayProvider.GetOrCreate(stack._elements.Length);
                Array.Copy(stack._elements, _elements, stack.Count);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = stack.Count;
            }
            else if (source is System.Collections.Generic.ICollection<T> collection)
            {
                _elements = ArrayProvider.GetOrCreate(collection.Count);
                collection.CopyTo(_elements, 0);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = collection.Count;
            }
            else
            {
                _elements = ArrayProvider.GetOrCreate(initialCapacity);
                foreach (var item in source)
                {
                    // ReSharper disable once VirtualMemberCallInConstructor
                    TryPush(item);
                }
            }
        }

        
        /// <inheritdoc cref="ICollection{T}"/>
        public override T[] ToArray()
        {
            var result = new T[Count];
            
            for (var i = 0; i < Count; i++)
            {
                result[i] = _elements[Count - i - 1];
            }
            
            return result;
        }

        /// <inheritdoc cref="ICollection{T}"/>
        public override bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                Count--;
                if (index < Count)
                {
                    Array.Copy(_elements, index + 1, _elements, index, Count - index);
                }

                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    _elements[Count] = default!;
                }

                return true;
            }

            return false;
        }

        private int IndexOf(T item)
        {
            for (var i = 0; i < Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(item, _elements[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override void Clear()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_elements, 0, Count);
            }
            
            Count = 0;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public override bool TryPush(T item)
        {
            if (Count == _elements.Length)
            {
                var tmp = ArrayProvider.GetOrCreate(_elements.Length << 1);
                Array.Copy(_elements, tmp, _elements.Length);
                ArrayProvider.Return(_elements);
                _elements = tmp;
            }

            _elements[Count] = item;
            Count++;
            return true;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public override bool TryPop([MaybeNull]out T item)
        {
            if (Count == 0)
            {
                item = default!;
                return false;
            }

            item = _elements[Count - 1];
            
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _elements[Count - 1] = default!;    
            }
            
            Count--;
            return true;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public override bool TryPeek([MaybeNull]out T item)
        {
            if (Count == 0)
            {
                item = default!;
                return false;
            }

            item = _elements[Count - 1];
            return true;
        }
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override IEnumerator<T> GetEnumerator()
        {
            return new ArrayStackEnumerator(this);
        }
        
        private struct ArrayStackEnumerator : IEnumerator<T>
        {
            private int _index;
            private T _current;
            private readonly ArrayStack<T> _stack;

            public ArrayStackEnumerator(ArrayStack<T> stack)
            {
                _stack = stack;
                _index = -2;
                _current = default!;
            }


            public bool MoveNext()
            {
                bool returnValue;
                switch (_index)
                {
                    case -2:
                    {
                        _index = _stack.Count - 1;
                        returnValue = _index >= 0;
                        if (returnValue)
                        {
                            _current = _stack._elements[_index];
                        }

                        return returnValue;
                    }
                    case -1:
                        return false;
                }

                returnValue = --_index >= 0;
                _current = returnValue ? _stack._elements[_index] : default!;
                return returnValue;
            }

            public void Reset()
            {
                _index = -2;
                _current = default!;
            }

            public T Current => _index switch
            {
                -2 => throw new InvalidOperationException("Enumerable not started"),
                -1 => throw new InvalidOperationException("Enumerable end"),
                _ => _current
            };

            object IEnumerator.Current => Current!;

            public void Dispose()
            {
                
            }
        }

        /// <summary>
        /// Creates a new <see cref="ArrayStack{T}"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="ArrayStack{T}"/> that is a copy of this instance.</returns>
        public ArrayStack<T> Clone() => new(this, ArrayProvider);

        object ICloneable.Clone() => Clone();
    }
}