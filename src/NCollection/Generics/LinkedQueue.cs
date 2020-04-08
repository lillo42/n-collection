using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NCollection.Generics.DebugViews;

namespace NCollection.Generics
{
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class LinkedQueue<T> : IQueue<T>, IQueue, ICloneable<LinkedQueue<T>>
    {
        private Node? _root;
        private Node? _last;
        public int Count { get; private set; }
        public bool IsSynchronized => false;
        public object SyncRoot => this;
        public bool IsReadOnly => false;

        public LinkedQueue()
        {
            
        }

        public LinkedQueue(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            
            foreach (var value in enumerable)
            {
                Enqueue(value);
            }
        }

        public void Enqueue(T item)
        {
            if (_root == null)
            {
                _last = _root = new Node(item);
            }
            else
            {
                _last!.Next = new Node(item);
                _last = _last!.Next;
            }

            Count++;
        }

        void IQueue.Enqueue(object? item)
            => Enqueue((T) item!);

        public T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        bool IQueue.TryPeek(out object? item)
        {
            if (TryPeek(out var value))
            {
                item = value;
                return true;
            }

            item = null;
            return false;
        }

        public bool TryPeek(out T item)
        {
            if (_root == null)
            {
                item = default!;
                return false;
            }

            item = _root.Value;
            return true;
        }

        public T Dequeue()
        {
            if (TryDequeue(out var value))
            {
                return value;
            }

            throw new InvalidOperationException("Empty queue");
        }

        bool IQueue.TryDequeue(out object? item)
        {
            if (TryDequeue(out var value))
            {
                item = value;
                return true;
            }

            item = null;
            return false;
        }

        public bool TryDequeue(out T item)
        {
            if (_root == null)
            {
                item = default!;
                return false;
            }
            
            item = _root.Value;
            _root = _root.Next;
            Count--;
            return true;
        }

        public LinkedQueueEnumerator GetEnumerator()
            => new LinkedQueueEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => GetEnumerator();

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

            if (_root != null)
            {
                var current = _root;
                for (var i = 0; current != null; i++)
                {
                    source[i] = current.Value;
                    current = current.Next!;
                } 
            }

            try
            {
                Array.Copy(source, 0, array, index, Count);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException("Invalid array type", nameof(array));
            }
        }

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
            
            if (_root != null)
            {
                var current = _root;
                for (var i = 0; current != null; i++)
                {
                    array[i + arrayIndex] = current.Value;
                    current = current.Next!;
                } 
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

        public bool Contains(T item)
        {
            var current = _root;

            while (current != null)
            {
                if (item == null)
                {
                    if (current.Value == null)
                    {
                        return true;
                    }
                }
                else if (current.Value != null && EqualityComparer<T>.Default.Equals(current.Value, item))
                {
                    return true;
                }
                
                current = current.Next;
            }

            return false;
        }

        bool ICollection.Contains(object? item)
            => Contains((T) item!);

        IQueue ICloneable<IQueue>.Clone()
        {
            return Clone();
        }

        public LinkedQueue<T> Clone() 
            => new LinkedQueue<T>(this);

        IQueue<T> ICloneable<IQueue<T>>.Clone()
            => Clone();

        object ICloneable.Clone()
            => Clone();

        public struct LinkedQueueEnumerator : IEnumerator<T>
        {
            private readonly LinkedQueue<T> _queue;
            private Node? _current;

            public LinkedQueueEnumerator(LinkedQueue<T> queue)
            {
                _queue = queue;
                _current = _queue._root;
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
                _current = _queue._root;
            }

            object? IEnumerator.Current => Current;

            public T Current { get; private set; }
            
            public void Dispose() { }
        }
    }
}