using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NCollection.Generics.DebugViews;

namespace NCollection.Generics
{
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ArrayQueue<T> : IQueue<T>, IQueue, ICloneable<ArrayQueue<T>>
    {
        private int _head;
        private int _tail;
        private T[] _array;

        public ArrayQueue()
        {
            _array = ArrayPool<T>.Shared.Rent(4);
        }

        public ArrayQueue(int initialCapacity)
        {
            _array = ArrayPool<T>.Shared.Rent(initialCapacity);
        }
        
        public ArrayQueue(IEnumerable<T> enumerable)
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
        

        public void Enqueue(T item)
        {
            if (Count == _array.Length)
            {
                var array = ArrayPool<T>.Shared.Rent(_array.Length * 2);
                if (_head < _tail)
                {
                    Array.Copy(_array, array, _array.Length);
                }
                else
                {
                    Array.Copy(_array, _head, array, 0, _array.Length - _head);
                    Array.Copy(_array, 0, array,  _array.Length - _head, _tail);
                }
                _head = 0;
                _tail = _array.Length;
                ArrayPool<T>.Shared.Return(array);
                _array = array;
            }
            
            _array[_tail] = item;
            _tail = (_tail + 1) % _array.Length; 
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
            if (Count == 0)
            {
                item = default!;
                return false;
            }
            
            item = _array[_head];
            return true;
        }
        
        
        public T Dequeue()
        {
            if (!TryDequeue(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
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
            if (Count == 0)
            {
                item = default!;
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
        
        
        public bool Contains(T item)
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
                else if (_array[size] != null && EqualityComparer<T>.Default.Equals(_array[size], item))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        bool ICollection.Contains(object? item) 
            => Contains((T) item!);

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

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Need no negative number");
            }

            if (arrayIndex > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Index out  of range");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Invalid length");
            }

            
            var size = _array.Length - _head < Count ? _array.Length - _head : Count;
            Array.Copy(_array, _head, array, arrayIndex, size);
            
            size = Count - size;
            if (size <= 0)
            {
                return;
            }
            
            Array.Copy(_array, 0, array, arrayIndex + _array.Length - _head, size);
        }

        IQueue ICloneable<IQueue>.Clone()
        {
            return Clone();
        }

        IQueue<T> ICloneable<IQueue<T>>.Clone() 
            => Clone();
        
        object ICloneable.Clone()
            => Clone();

        public ArrayQueue<T> Clone()
        {
            var queue = new ArrayQueue<T>(_array.Length);
            Array.Copy(_array, queue._array, _array.Length);
            queue.Count = Count;
            queue._head = _head;
            queue._tail = _tail;
            return queue;
        }
        
        public QueueStackEnumerator GetEnumerator() 
            => new QueueStackEnumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public struct QueueStackEnumerator : IEnumerator<T>
        {
            private int _index;
            private T _current;
            private readonly ArrayQueue<T> _queue;
            public QueueStackEnumerator(ArrayQueue<T> queue)
            {
                _queue = queue;
                _index = queue.Count == 0 ?  0 : -2;
                _current = default!;
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
                _current = returnValue ? _queue._array[(_queue._head + _index) % _queue._array.Length] : default!;
                return returnValue;
            }

            public void Reset()
            {
                _index = -2;
                _current = default!;
            }

            public T Current => _index switch
            {
                -2 => throw new InvalidOperationException("Enumerable not started"),
                -1 => throw new InvalidOperationException("Enumerable end"),
                _ => _current
            };


            object? IEnumerator.Current
                => Current;
            
            public void Dispose() { }
        }
    }
}