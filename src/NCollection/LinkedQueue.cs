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
        private int _count;

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
        /// <exception cref="ArgumentNullException">if the <see cref="source"/> is null </exception>
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
        
        /// <inheritdoc cref="ICollection{T}"/>
        public override int Count => _count;
        

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

            _count++;
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
            _count--;
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
            private Node? _head;
            private int _state;
            private readonly LinkedQueue<T> _queue;

            public QueueEnumerator(LinkedQueue<T> queue)
            {
                _queue = queue;
                _head = null;
                _state = -1;
                Current = default!;
            }

            public bool MoveNext()
            {
                switch (_state)
                {
                    case -1:
                        _head = _queue._head;
                        _state = 0;
                        if (_head == null)
                        {
                            _state = -2;
                            goto case -2;
                        }
                        break;
                    case 0:
                        _head = _head?.Next;
                        if (_head == null)
                        {
                            _state = -2;
                            goto case -2;
                        }
                        break;
                    case -2:
                        Current = default!;
                        return false;
                }

                Current = _head!.Value;
                return true;
            }

            public void Reset()
            {
                _head = null;
                _state = -1;
                Current = default!;
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
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