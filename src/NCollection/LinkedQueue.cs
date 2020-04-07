using System;
using System.Collections;
using System.Diagnostics;
using NCollection.DebugViews;

namespace NCollection
{
    [DebuggerTypeProxy(typeof(ICollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class LinkedQueue : IQueue, ICloneable<LinkedQueue>
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

        public LinkedQueue(IEnumerable enumerable)
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

        public void Enqueue(object? item)
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

        public object? Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        public bool TryPeek(out object? item)
        {
            if (_root == null)
            {
                item = null;
                return false;
            }

            item = _root.Value;
            return true;
        }

        public object? Dequeue()
        {
            if (TryDequeue(out var value))
            {
                return value;
            }

            throw new InvalidOperationException("Empty queue");
        }

        public bool TryDequeue(out object? item)
        {
            if (_root == null)
            {
                item = null;
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

        private class Node
        {
            public Node(object? value)
            {
                Value = value;
            }

            public object? Value { get; }
            public Node? Next { get; set; }
        }

        public bool Contains(object? item)
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
                else if (current.Value != null && current.Value.Equals(item))
                {
                    return true;
                }
                
                current = current.Next;
            }

            return false;
        }

        public LinkedQueue Clone() 
            => new LinkedQueue(this);

        IQueue ICloneable<IQueue>.Clone()
            => Clone();

        object ICloneable.Clone()
            => Clone();

        public struct LinkedQueueEnumerator : IEnumerator
        {
            private readonly LinkedQueue _queue;
            private Node? _current;

            public LinkedQueueEnumerator(LinkedQueue queue)
            {
                _queue = queue;
                _current = _queue._root;
                Current = null;
            }

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

            public void Reset()
            {
                _current = _queue._root;
            }

            public object? Current { get; private set; }
        }
    }
}