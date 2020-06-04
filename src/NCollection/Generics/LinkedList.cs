using System;
using System.Collections;
using System.Collections.Generic;

namespace NCollection.Generics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkedList<T> : IList<T>, IList
    {
        private Node? _first;
        private Node? _last;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedList{T}"/>.
        /// </summary>
        public LinkedList()
        {
            
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedList{T}"/>.List
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public LinkedList(IEnumerable<T> enumerable)
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


        

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public int Count { get; private set; }

        /// <inheritdoc />
        public bool IsSynchronized => false;

        /// <inheritdoc />
        public object SyncRoot => this;

        bool System.Collections.IList.IsFixedSize => false;

        /// <inheritdoc cref="ICollection{T}"/>
        public bool IsReadOnly => false;

        object? System.Collections.IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T) value!;
        }

        /// <inheritdoc />
        public T this[int index]
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
            
            Node? current;
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
                for (var i = Count - 1; i > index; i++)
                {
                    current = current!.Preview;
                }
            }

            return current!;
        }

        /// <inheritdoc cref="ICollection{T}"/>
        public bool IsEmpty => _first == null;

        /// <inheritdoc />
        public void Add(T item)
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

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}" />
        public void Clear()
        {
            var current = _first;

            while (current != null)
            {
                var next = current.Next;
                current.Preview = null;
                current.Value = default!;
                current.Next = null;
                
                current = next;
            }

            Count = 0;
        }

        int System.Collections.IList.IndexOf(object value)
            => IndexOf((T) value!);

        void System.Collections.IList.Insert(int index, object value)
            => Insert(index, (T) value!);

        /// <inheritdoc />
        public bool Contains(T item)
        {
            var node = _first;
            
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
                
                node = node.Next;
            } 

            return false;
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("Rank multi-dimensional not supported", nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Index out  of range");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Invalid Off length");
            }

            
            var source = new T[Count];
            
            var current = _first;
            for (var i = 0; current != null; i++)
            {
                source[i] = current.Value;
                current = current.Next;
            }

            try
            {
                Array.Copy(source, 0, array, arrayIndex, Count);
            }
            catch (ArrayTypeMismatchException ex)
            {
                throw new ArgumentException("Invalid array type", nameof(array), ex);
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

            
            var source = new object[Count];
            
            var current = _first;
            for (var i = 0; current != null; i++)
            {
                source[i] = current.Value!;
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

        /// <inheritdoc />
        public bool Remove(T item)
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
                else if (current.Value != null && EqualityComparer<T>.Default.Equals(current.Value, item))
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
            node.Value = default!;
            Count--;
        }

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public void RemoveAt(int index)
            => Remove(GetNode(index));

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            var node = _first;
            var counter = 0;
            
            while (node != null)
            {
                if (item == null)
                {
                    if (node.Value == null)
                    {
                        return counter;
                    }
                }
                else if (node.Value != null && EqualityComparer<T>.Default.Equals(node.Value, item))
                {
                    return counter;
                }
                
                node = node.Next;
                counter++;
            } 

            return -1;
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            var current = GetNode(index);
            
            var node = new Node(current!.Preview, current.Next, item);
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
        public void Sort(ISortAlgorithm<T> algorithm, IComparer<T> comparer)
        {
            var array = new T[Count];
            CopyTo(array, 0);
            
            algorithm.Execute(array, comparer);

            var current = _first;
            foreach (var value in array)
            {
                current!.Value = value;
                current = current.Next;
            }
        }

        /// <summary>
        /// Creates a shallow copy of the <see cref="LinkedList"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="LinkedList"/>.</returns>
        public LinkedList<T> Clone() 
            => new LinkedList<T>(this);
        
        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Returns an <see cref="LinkedLinkEnumerator"/> for the <see cref="LinkedList"/>.
        /// </summary>
        /// <returns>An <see cref="LinkedLinkEnumerator"/>  for the <see cref="LinkedList"/>.</returns>
        public LinkedLinkEnumerator GetEnumerator() 
            => new LinkedLinkEnumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        #region IList

        void IList.Sort(ISortAlgorithm algorithm, IComparer comparable)
        {
            if (Count == 0)
            {
                return;
            }
            
            var obj = new object[Count];
            CopyTo(obj, 0);
            algorithm.Execute(obj, comparable);

            var current = _first;
            foreach (var value in obj)
            {
                current!.Value = (T) value;
                current = current.Next;
            }
            
        }

        bool IList.Contains(object? item)
            => Contains((T) item!);

        void IList.Add(object? item)
            => Add((T) item!);

        bool IList.Remove(object? item)
            => Remove((T) item!);

        #endregion

        private sealed class Node
        {
            public Node(Node? preview, Node? next, T value)
            {
                Preview = preview;
                Next = next;
                Value = value;
            }

            public Node? Preview { get; set; }
            public Node? Next { get; set; }
            public T Value { get; set; }
        }
        
        /// <summary>
        /// Implementation of <see cref="IEnumerable{T}"/> for <see cref="LinkedList{T}"/>.
        /// </summary>
        public struct LinkedLinkEnumerator : IEnumerator<T>
        {
            private readonly LinkedList<T> _list;
            private Node? _current;

            /// <summary>
            /// Initializes a new instance of the <see cref="LinkedLinkEnumerator"/>.
            /// </summary>
            /// <param name="list">The <see cref="LinkedQueue"/> queue.</param>
            public LinkedLinkEnumerator(LinkedList<T> list)
            {
                _list = list;
                _current = _list._first;
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
                _current = _list._first;
            }

            object IEnumerator.Current => Current! ;

            /// <inheritdoc />
            public T Current { get; private set; }

            /// <inheritdoc />
            public void Dispose()
            {
                
            }
        }
    }
}
