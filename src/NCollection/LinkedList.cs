using System;
using System.Collections;
using System.Diagnostics;
using NCollection.DebugViews;

namespace NCollection
{
    /// <summary>
    ///  The link listed
    /// </summary>
    [DebuggerTypeProxy(typeof(ICollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class LinkedList : IList
    {
        private Node? _first;
        private Node? _last;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedList"/>.
        /// </summary>
        public LinkedList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedList"/>.List
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public LinkedList(IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            
            foreach (var value in enumerable)
            {
                Add(value);
            }
        }
        
        /// <inheritdoc cref="ICollection"/>
        public int Count { get; private set; }
        
        /// <inheritdoc />
        public bool IsSynchronized => false;
        
        /// <inheritdoc />
        public object SyncRoot => this;
        
        /// <inheritdoc />
        public bool IsEmpty => _first == null;
        
        /// <inheritdoc cref="IList"/>
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public object? this[int index]
        {
            get => GetNode(index).Value;
            set
            {
                var current = GetNode(index);
                current!.Value = value;
            }
        }
        
        private Node GetNode(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),  "index out of range");
            }
            
            Node? current = null;
            if (index <= Count / 2)
            { 
                current = _first;
                for (var i = 0; i < index; i++)
                {
                    current = current!.Next;
                }
            }
            else
            {
                current = _last;
                for (var i = Count - 1; i > index; i--)
                {
                    current = current!.Preview;
                }
            }

            return current!;
        }

        /// <inheritdoc />
        public bool IsFixedSize => false;

        /// <inheritdoc cref="IList"/>
        public void Add(object? item)
        {
            if (_first == null)
            {
                _last = _first = new Node(null, null, item);
                Count++;
            }
            else
            {
                var last = new Node(_last, null, item);
                _last!.Next = last;
                _last = last;
                Count++;
            }
        }
        
        /// <inheritdoc cref="IList"/>
        public void Clear()
        {
            var current = _first;

            while (current != null)
            {
                var next = current.Next;
                current.Preview = null;
                current.Value = null;
                current.Next = null;
                
                current = next;
            }

            Count = 0;
        }

        /// <inheritdoc cref="IList"/>
        public bool Remove(object? item)
        {
            var current = _first;
            
            while (current != null)
            {
                if (item == null)
                {
                    if (current.Value == null)
                    {
                        Remove(current);
                        return true;
                    }
                }
                else if (current.Value != null && current.Value.Equals(item))
                {
                    Remove(current);
                    return true;
                }
                
                current = current.Next;
            } 

            return false;
        }
        
        private void Remove(Node node)
        {
            var next = node.Next;
            var preview = node.Preview;

            if (next != null)
            {
                next.Preview = preview;
            }
            else
            {
                _last = preview;
            }

            if (preview != null)
            {
                preview.Next = next;
            }
            else
            {
                _first = next;
            }
                        
            node.Next = null;
            node.Preview = null;
            node.Value = null;
            Count--;
        }

        /// <inheritdoc cref="IList"/>
        public bool Contains(object? value)
        {
            var node = _first;
            
            while (node != null)
            {
                if (value == null)
                {
                    if (node.Value == null)
                    {
                        return true;
                    }
                }
                else if (node.Value != null && node.Value.Equals(value))
                {
                    return true;
                }
                
                node = node.Next;
            } 

            return false;
        }

        /// <inheritdoc />
        public int IndexOf(object value)
        {
            var node = _first;
            var counter = 0;
            
            while (node != null)
            {
                if (value == null)
                {
                    if (node.Value == null)
                    {
                        return counter;
                    }
                }
                else if (node.Value != null && node.Value.Equals(value))
                {
                    return counter;
                }
                
                node = node.Next;
                counter++;
            } 

            return -1;
        }

        /// <inheritdoc />
        public void Insert(int index, object value)
        {
            var current = GetNode(index);
            
            var node = new Node(current!.Preview, current.Next, value);
            var next = current.Next;
            var preview = current.Preview;

            if (next != null)
            {
                next.Preview = node;
            }
            else
            {
                _last = node;
            }

            if (preview != null)
            {
                preview.Next = node;
            }
            else
            {
                _first = node;
            }

            Count++;
        }

        /// <inheritdoc />
        public void RemoveAt(int index) 
            => Remove(GetNode(index));

        /// <inheritdoc />
        public void Sort(ISortAlgorithm algorithm, IComparer comparable)
        {
            var array = new object[Count];
            CopyTo(array, 0);
            
            algorithm.Sort(array, comparable);

            var current = _first;
            foreach (var value in array)
            {
                current!.Value = value;
                current = current.Next;
            }
        }

        
        /// <inheritdoc />
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
            
            var current = _first;
            for (var i = 0; current != null; i++)
            {
                source[i] = current.Value;
                current = current.Next;
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
        
        /// <summary>
        /// Returns an <see cref="LinkedLinkEnumerator"/> for the <see cref="LinkedList"/>.
        /// </summary>
        /// <returns>An <see cref="LinkedLinkEnumerator"/>  for the <see cref="LinkedList"/>.</returns>
        public LinkedLinkEnumerator GetEnumerator() 
            => new LinkedLinkEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        /// <summary>
        /// Creates a shallow copy of the <see cref="LinkedList"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="LinkedList"/>.</returns>
        public LinkedList Clone() 
            => new LinkedList(this);

        object ICloneable.Clone()
            => Clone();
        
        private sealed class Node
        {
            public Node(Node? preview, Node? next, object? value)
            {
                Preview = preview;
                Next = next;
                Value = value;
            }

            public Node? Preview { get; set; }
            public Node? Next { get; set; }
            public object? Value { get; set; }
        }
        
        /// <summary>
        /// Implementation of <see cref="IEnumerable"/> for <see cref="LinkedList"/>.
        /// </summary>
        public struct LinkedLinkEnumerator : IEnumerator
        {
            private readonly LinkedList _list;
            private Node? _current;

            /// <summary>
            /// Initializes a new instance of the <see cref="LinkedLinkEnumerator"/>.
            /// </summary>
            /// <param name="list">The <see cref="LinkedQueue"/> queue.</param>
            public LinkedLinkEnumerator(LinkedList list)
            {
                _list = list;
                _current = _list._first;
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
                _current = _current.Next;
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _current = _list._first;
            }

            /// <inheritdoc />
            public object? Current { get; private set; }
        }
    }
}