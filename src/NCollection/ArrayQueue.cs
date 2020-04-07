using System;
using System.Buffers;
using System.Collections;
using System.Diagnostics;
using NCollection.DebugViews;

namespace NCollection
{
    [DebuggerTypeProxy(typeof(ICollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ArrayQueue : IQueue, ICloneable<ArrayQueue>
    {
        private int _head;
        private int _tail;
        private object?[] _array;

        public ArrayQueue()
        {
            _array = ArrayPool<object>.Shared.Rent(4);
        }

        public ArrayQueue(int initialCapacity)
        {
            _array = ArrayPool<object>.Shared.Rent(initialCapacity);
        }
        
        public ArrayQueue(IEnumerable enumerable)
            : this()
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

        public int Count { get; private set; }
        public bool IsSynchronized => false;
        public object SyncRoot => this;
        public bool IsReadOnly => false;
        
        public void Enqueue(object? item)
        {
            if (Count == _array.Length)
            {
                var array = ArrayPool<object>.Shared.Rent(_array.Length * 2);
                if (_head < _tail)
                {
                    Array.Copy(_array, array, _array.Length);
                }
                else
                {
                    Array.Copy(_array, _head, array, 0, _array.Length - _head);
                    Array.Copy(_array, 0, array,  _array.Length - _head, _tail);
                }
                
                ArrayPool<object>.Shared.Return(array);
                _array = array;
            }
            
            _array[_tail] = item;
            _tail = (_tail + 1) % _array.Length; 
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
            if (Count == 0)
            {
                item = null;
                return false;
            }
            
            item = _array[_head];
            return true;
        }
        
        
        public object? Dequeue()
        {
            if (!TryDequeue(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }
        
        public bool TryDequeue(out object? item)
        {
            if (Count == 0)
            {
                item = null;
                return false;
            }
            
            item = _array[_head];
            _head = (_head + 1) % _array.Length;
            Count--;
            return true;
        }

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

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Need no negative number");
            }

            if (index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index out  of range");
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException("Invalid length");
            }

            
            var size = _array.Length - _head < Count ? _array.Length - _head : Count;
            Array.Copy(_array, _head, array, index, size);
            
            size = Count - size;
            if (size <= 0)
            {
                return;
            }
            
            Array.Copy(_array, 0, array, index + _array.Length - _head, size);
        }
        
        
        public bool Contains(object? item)
        {
            var size = -1;
            while (++size < Count)
            {
                if (item == null)
                {
                    if (_array[size] == null)
                    {
                        return true;
                    }
                }
                else if (_array[size] != null && _array[size]!.Equals(item))
                {
                    return true;
                }
            }
            
            return false;
        }

        IQueue ICloneable<IQueue>.Clone() 
            => Clone();
        
        object ICloneable.Clone()
            => Clone();

        public ArrayQueue Clone()
        {
            var queue = new ArrayQueue(_array.Length);
            Array.Copy(_array, queue._array, _array.Length);
            queue.Count = Count;
            queue._head = _head;
            queue._tail = _tail;
            return queue;
        }


        public QueueStackEnumerator GetEnumerator() 
            => new QueueStackEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public struct QueueStackEnumerator : IEnumerator
        {
            private int _index;
            private object? _current;
            private readonly ArrayQueue _queue;
            public QueueStackEnumerator(ArrayQueue queue)
            {
                _queue = queue;
                _index = queue.Count == 0 ?  0 : -2;
                _current = null;
            }

            public bool MoveNext()
            {
                switch (_index)
                {
                    case -2:
                        _index = -1;
                        break;
                    case -1:
                        return false;
                }
                
                var returnValue = ++_index < _queue.Count;
                _current = returnValue ? _queue._array[(_queue._head + _index) % _queue._array.Length] : null;
                return returnValue;
            }

            public void Reset()
            {
                _index = -2;
                _current = null;
            }

            public object? Current 
                => _index switch
                {
                    -2 => throw new InvalidOperationException("Enumerable not started"),
                    -1 => throw new InvalidOperationException("Enumerable end"),
                    _ => _current
                };
        }
    }
}