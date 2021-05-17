using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCollection
{
    /// <summary>
    /// The implementation of <see cref="IList{T}"/> using linked node.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class LinkedList<T> : AbstractList<T>, ICloneable
    {
        private Node? _head;
        private Node? _tail;

        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public override T this[int index]
        {
            get => ElementAt(index).Value;
            set => ElementAt(index).Value = value;
        }

        /// <inheritdoc cref="IList{T}"/>
        public override IList<T> this[Range range]
        {
            get
            {
                var linked = new LinkedList<T>();
                var item = ElementAt(range.Start.Value);
                var position = range.Start.Value;

                while (item != null && (range.End.IsFromEnd || position < range.End.Value))
                {
                    linked.Add(item.Value);
                    item = item.Next;
                    position++;
                }

                return linked;
            }
        }

        /// <summary>
        /// Initialize <see cref="LinkedList{T}"/>
        /// </summary>
        public LinkedList()
        {
            
        }

        /// <summary>
        /// Initialize <see cref="LinkedList{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> is <see langword="null"/></exception>
        public LinkedList(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            foreach (var item in source)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                TryAdd(item);
            }
        }
        
        private Node ElementAt(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "The index should be greater than 0");
            }

            if (index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"The index should be less than {Count}");
            }
            
            if (index <= Count / 2)
            {
                var current = _head;
                var position = 0;
                while (current != null)
                {
                    if (position == index)
                    {
                        return current;
                    }
                    current = current.Next;
                    position++;
                }
            }
            else
            {
                var current = _tail;
                var position = Count - 1;
                while (current != null)
                {
                    if (position == index)
                    {
                        return current;
                    }
                    current = current.Previous;
                    position--;
                }
            }
            
            throw new InvalidOperationException("Collection was modify while the search, item not found");
        }

        /// <inheritdoc cref="ICollection{T}"/>
        public override bool TryAdd(T item)
        {
            var node = new Node(item, _tail, null);
            _head ??= node;
            
            if (_tail != null)
            {
                _tail.Next = node;
            }

            _tail = node;
            Count++;
            return true;
        }

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override bool Remove(T item)
        {
            var current = _head;
            while (current != null)
            {
                if(EqualityComparer<T>.Default.Equals(current.Value, item))
                {
                    Remove(current);
                    return true;
                }
                
                current = current.Next;
            }

            return false;
        }
        
        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public override void RemoveAt(int index)
        {
            Remove(ElementAt(index));
        }

        private void Remove(Node node)
        {
            if (_head == node)
            {
                _head = node.Next;
            }
            
            if (_tail == node)
            {
                _tail = node.Previous;
            }
            
            var next = node.Next;
            var prev = node.Previous;

            if (next != null)
            {
                next.Previous = prev;
            }

            if (prev != null)
            {
                prev.Next = next;
            }
            
            Count--;
        }
        
        /// <inheritdoc cref="IList{T}"/>
        public override bool AddAll(int index, IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            var next = ElementAt(index);
            var current = next.Previous;
            var modified = false;
            var first = index == 0;

            foreach (var item in source)
            {
                var node = new Node(item, current, null);

                if (first)
                {
                    _head = node;
                    first = false;
                }
                
                if (current != null)
                {
                    current.Next = node;
                }
                
                current = node;
                Count++;
                
                modified = true;
            }

            if (current != null)
            {
                current.Next = next;
                next.Previous = current;
            }

            return modified;
        }
        
        /// <inheritdoc cref="IList{T}"/>
        public override void Add(int index, T item)
        {
            var current = ElementAt(index);
            var node = new Node(item, current.Previous, current);
            
            if (current.Previous != null)
            {
                current.Previous.Next = node;
            }
            
            current.Previous = node;

            if (index == 0)
            {
                _head = node;
            }
            else if (index == Count - 1)
            {
                _tail = node;
            }

            Count++;
        }
        
        /// <inheritdoc cref="IList{T}"/>
        public override int IndexOf(T item, IEqualityComparer<T> comparer)
        {
            var index = 0;
            var current = _head;

            while (current != null)
            {
                if (comparer.Equals(current.Value, item))
                {
                    return index;
                }

                current = current.Next;
                index++;
            }

            return -1;
        }
        
        /// <inheritdoc cref="IList{T}"/>
        public override int LastIndexOf(T item, IEqualityComparer<T> comparer)
        {
            var index = Count - 1;
            var current = _tail;

            while (current != null)
            {
                if (comparer.Equals(current.Value, item))
                {
                    return index;
                }

                current = current.Previous;
                index--;
            }

            return -1;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            Count = 0;
            _tail = null;
            
            var current = _head;
            while (current != null)
            {
                if (current.Previous != null)
                {
                    current.Previous.Next = null;
                }

                current.Previous = null;
                current = current.Next;
            }

            _head = null;
        }

        /// <inheritdoc />
        public override IEnumerator<T> GetEnumerator()
        {
            return new LinkedListEnumerator(this);
        }

        /// <summary>
        /// Creates a new <see cref="LinkedList{T}"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="LinkedList{T}"/> that is a copy of this instance.</returns>
        public LinkedList<T> Clone()
        {
            return new LinkedList<T>(this);
        }

        object ICloneable.Clone()
            => Clone();
        
        private class Node
        {
            public Node(T value, Node? previous, Node? next)
            {
                Value = value;
                Previous = previous;
                Next = next;
            }

            public T Value { get; set; }
            public Node? Previous { get; set; }
            public Node? Next { get; set; }
        }
        
        private struct LinkedListEnumerator : IEnumerator<T>
        {
            private readonly LinkedList<T> _list;
            private Node? _current;

            public LinkedListEnumerator(LinkedList<T> list)
            {
                _list = list;
                _current = _list._head;
                Current = default!;
            }


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

            public void Reset()
            {
                _current = _list._head;
            }

            public T Current { get; private set; }

            object? IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }
        }
    }
}