using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    /// The implementation of <see cref="IQueue{T}"/> using linked node.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class LinkedQueue<T> : AbstractQueue<T>, ICloneable
    {
        private Node? _head;
        private Node? _tail;

        /// <summary>
        /// Initialize <see cref="LinkedQueue{T}"/>
        /// </summary>
        public LinkedQueue()
        {
            
        }
        
        /// <summary>
        /// Initialize <see cref="LinkedQueue{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <exception cref="ArgumentNullException">if the <see cref="source"/> is <see langword="null"/></exception>
        public LinkedQueue([JetBrains.Annotations.NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var item in source)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                TryEnqueue(item);
            }
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public override bool TryEnqueue(T item)
        {
            if (_head == null)
            {
                _head = _tail = new Node(item);
            }
            else
            {
                _tail!.Next = new Node(item);
                _tail = _tail.Next;
            }

            Count++;
            return true;
        }

        /// <inheritdoc cref="IQueue{T}"/>
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

        /// <inheritdoc cref="IQueue{T}"/>
        public override bool TryDequeue([MaybeNull]out T item)
        {
            if (_head == null)
            {
                item = default;
                return false;
            }

            item = _head.Value;
            _head = _head.Next;
            Count--;
            return true;
        }

        /// <summary>
        /// Creates a new <see cref="LinkedQueue{T}"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="LinkedQueue{T}"/> that is a copy of this instance.</returns>
        public LinkedQueue<T> Clone()
        {
            return new LinkedQueue<T>(this);
        }
        
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override IEnumerator<T> GetEnumerator()
        {
            return new QueueEnumerator(this);
        }

        private struct QueueEnumerator : IEnumerator<T>
        {
            private readonly LinkedQueue<T> _queue;
            private Node? _current;

            /// <summary>
            /// Initializes a new instance of the <see cref="QueueEnumerator"/>.
            /// </summary>
            /// <param name="queue">The <see cref="LinkedQueue{T}"/>.</param>
            public QueueEnumerator(LinkedQueue<T> queue)
            {
                _queue = queue;
                _current = _queue._head;
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
                _current = _queue._head;
            }

            object? IEnumerator.Current => Current;

            /// <inheritdoc />
            public T Current { get; private set; }
            
            /// <inheritdoc />
            public void Dispose() { }
        }
        
        private class Node
        {
            public Node(T value)
            {
                Value = value;
            }

            public T Value { get; }
            public Node? Next { get; set; }
        }
        
    }
}