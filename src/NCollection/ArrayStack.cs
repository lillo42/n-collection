using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    /// The implementation of <see cref="IStack{T}"/> using array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ArrayStack<T> : AbstractStack<T>, ICloneable
    {
        private T[] _elements;
        private int _count;
        
        /// <inheritdoc cref="ICollection{T}"/>
        public override int Count => _count;

        /// <summary>
        /// Initialize <see cref="ArrayStack{T}"/>
        /// </summary>
        public ArrayStack()
        {
            _elements = ArrayPool<T>.Shared.Rent(16);
        }

        /// <summary>
        /// Initialize <see cref="ArrayStack{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <exception cref="ArgumentNullException">if the <see cref="source"/> is null </exception>
        public ArrayStack([JetBrains.Annotations.NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ArrayStack<T> stack)
            {
                _elements = ArrayPool<T>.Shared.Rent(stack._elements.Length);
                Array.Copy(stack._elements, _elements, stack._count);
            }
            else if (source is System.Collections.Generic.ICollection<T> collection)
            {
                _elements = ArrayPool<T>.Shared.Rent(collection.Count);
                collection.CopyTo(_elements, 0);
            }
            else
            {
                _elements = ArrayPool<T>.Shared.Rent(16);
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
            var result = new T[_count];
            Array.Copy(_elements, result, result.Length);
            return result;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public override bool TryPush(T item)
        {
            if (_count == _elements.Length)
            {
                var tmp = ArrayPool<T>.Shared.Rent(_elements.Length >> 1);
                Array.Copy(_elements, tmp, _elements.Length);
                ArrayPool<T>.Shared.Return(_elements, true);
                _elements = tmp;
            }

            _elements[_count++] = item;
            return true;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public override bool TryPop([MaybeNull]out T item)
        {
            if (_count == 0)
            {
                item = default!;
                return false;
            }

            item = _elements[_count];
            _elements[_count--] = default!;
            return true;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public override bool TryPeek([MaybeNull]out T item)
        {
            if (_count == 0)
            {
                item = default!;
                return false;
            }

            item = _elements[_count];
            return true;
        }
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override IEnumerator<T> GetEnumerator()
        {
            return new ArrayStackEnumerator(this);
        }
        
        private struct ArrayStackEnumerator : IEnumerator<T>
        {
            private int _head;
            private int _state;
            private readonly ArrayStack<T> _stack;

            public ArrayStackEnumerator(ArrayStack<T> stack)
            {
                _stack = stack;
                _head = 0;
                _state = -1;
                Current = default!;
            }


            public bool MoveNext()
            {
                switch (_state)
                {
                    case  -1:
                        _head = _stack._count;
                        _state = 0;
                        break;
                    case 0:
                        _head--;
                        if (_head == -1)
                        {
                            _state = -2;
                            goto case -2;
                        }
                        break;
                    case -2:
                        return false;
                    
                }

                Current = _stack._elements[_head];
                return true;
            }

            public void Reset()
            {
                _state = -1;
                Current = default!;
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current!;

            public void Dispose()
            {
                
            }
        }

        /// <summary>
        /// Creates a new <see cref="ArrayStack{T}"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="ArrayStack{T}"/> that is a copy of this instance.</returns>
        public ArrayStack<T> Clone() => new ArrayStack<T>(this);

        object ICloneable.Clone() => Clone();
    }
}