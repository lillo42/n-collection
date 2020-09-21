using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    /// The implementation of <see cref="IStack{T}"/> using linked node.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class LinkedStack<T> : AbstractStack<T>, ICloneable
    {
        /// <summary>
        /// Initialize <see cref="LinkedStack{T}"/>
        /// </summary>
        public LinkedStack()
        {
            
        }

        /// <summary>
        /// Initialize <see cref="LinkedStack{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> is null </exception>
        public LinkedStack([JetBrains.Annotations.NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var item in source)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                TryPush(item);
            }
        }
        
        
        private Node? _head;

        /// <inheritdoc cref="IStack{T}"/>
        public override bool TryPush(T item)
        {
            var head = new Node(item, _head);
            _head = head;
            Count++;
            return true;
        }
        

        /// <inheritdoc cref="IStack{T}"/>
        public override bool TryPop([MaybeNull]out T item)
        {
            if (_head == null)
            {
                item = default;
                return false;
            }

            item = _head.Value;
            _head = _head.Previous;
            Count--;
            return true;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public override bool TryPeek([MaybeNull]out T item)
        {
            if (_head == null)
            {
                item = default; 
                return false;
            }
            
            item = _head.Value;
            return true;
        }

        /// <inheritdoc cref="ICollection{T}"/>
        public override bool Remove(T item)
        {
            var current = _head;
            Node? next = null;

            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(item, current.Value))
                {
                    if (next != null)
                    {
                        next.Previous = current.Previous;
                    }

                    if (current == _head)
                    {
                        _head = current.Previous;
                    }
                    
                    current.Previous = null;
                    Count--;
                    return true;
                }

                next = current;
                current = current.Previous;
            }

            return false;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            var current = _head;
            while (current != null)
            {
                var prev = current;
                current = current.Previous;
                prev.Previous = null;
            }

            Count = 0;
        }

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override IEnumerator<T> GetEnumerator()
        {
            return new StackEnumerator(this);
        }
        
        private struct StackEnumerator : IEnumerator<T>
        {
            private readonly LinkedStack<T> _stack;
            private Node? _current;

            /// <summary>
            /// Initializes a new instance of the <see cref="StackEnumerator"/>.
            /// </summary>
            /// <param name="stack">The <see cref="LinkedStack{T}"/>.</param>
            public StackEnumerator(LinkedStack<T> stack)
            {
                _stack = stack;
                _current = _stack._head;
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
                _current = _current.Previous;
                return true;
            }

            /// <inheritdoc />
            public void Reset() 
                => _current = _stack._head;
            
            object? IEnumerator.Current => Current;

            /// <inheritdoc />
            public T Current { get; private set; }
            
            /// <inheritdoc />
            public void Dispose()
            {
            }
        }
        
        private class Node
        {
            public Node(T value, Node? previous)
            {
                Value = value;
                Previous = previous;
            }

            public T Value { get; }

            public Node? Previous { get; set; }
        }
        
        /// <summary>
        /// Creates a new <see cref="LinkedStack{T}"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="LinkedStack{T}"/> that is a copy of this instance.</returns>
        public LinkedStack<T> Clone() => new LinkedStack<T>(this);

        object ICloneable.Clone()
            => Clone();
    }
}