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
    /// <typeparam name="T"></typeparam>
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
        /// <exception cref="ArgumentNullException">if the <see cref="source"/> is null </exception>
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
        private int _count;
        
        /// <inheritdoc cref="ICollection{T}"/>
        public override int Count => _count;

        /// <inheritdoc cref="IStack{T}"/>
        public override bool TryPush(T item)
        {
            var head = new Node(item, _head);
            _head = head;
            _count++;
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
            _count--;
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
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override IEnumerator<T> GetEnumerator()
        {
            return new StackEnumerator(this);
        }
        
        private struct StackEnumerator : IEnumerator<T>
        {
            private readonly LinkedStack<T> _stack;
            private int _state;
            private Node? _current;
            public StackEnumerator(LinkedStack<T> stack)
            {
                _stack = stack;
                _state = -1;
                _current = null;
                Current = default!;
            }

            public bool MoveNext()
            {
                switch (_state)
                {
                    case -2:
                        return false;
                    case -1:
                        _state = 0;
                        _current = _stack._head;
                        return true;
                    case 0:
                        _current = _current?.Previous;
                        if (_current == null)
                        {
                            Current = default!;
                            _state = -2;
                            return false;
                        }
                        else
                        {
                            Current = _current.Value;
                        }
                        
                        return true;
                }

                return false;
            }

            public void Reset()
            {
                _current = null;
                Current = default!;
                _state = -1;
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current!;

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

            public Node? Previous { get; }
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