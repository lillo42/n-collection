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
    public class ArrayStack : IStack, ICloneable<ArrayStack>
    {
        private int _size = 0;
        private object?[] _array;
        
        public ArrayStack()
        {
            _array = ArrayPool<object>.Shared.Rent(4);
        }

        public ArrayStack(int initialCapacity)
        {
            _array = new object[initialCapacity];
        }
        
        public ArrayStack(IEnumerable enumerable)
            : this()
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Push(enumerator.Current);
            }
        }
        
        public virtual int Count => _size;

        public virtual bool IsSynchronized => false;

        public virtual object SyncRoot => this;

        public virtual bool IsReadOnly => false;
        
        public virtual void CopyTo(Array array, int index)
        {
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

            if (array.Length - index < _size)
            {
                throw new ArgumentException("Invalid length");
            }

            if (array is object?[] objArray)
            {
                for (var i = 0; i < _size; i++)
                {
                    objArray[i + index] = _array[_size - i - 1];
                }
            }
            else
            {
                for (var i = 0; i < _size; i++)
                {
                    array.SetValue(_array[_size - i - 1], i + index);
                }
            }
        }

        public virtual bool Contains(object? item)
        {
            var size = _size;
            while (size-- > 0)
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
        
        public virtual object? Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }
        
        public virtual bool TryPeek(out object? item)
        {
            if (_size == 0)
            {
                item = null;
                return false;
            }

            item = _array[_size - 1];
            return true;
        }

        public virtual void Push(object? item)
        {
            if (_size == _array.Length)
            {
                var array = ArrayPool<object>.Shared.Rent(_array.Length * 2);
                Array.Copy(_array, array, _array.Length);
                ArrayPool<object>.Shared.Return(_array!);
                _array = array;
            }

            _array[_size] = item;
            _size++;
        }

        public virtual object? Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }
        
        public virtual bool TryPop(out object? item)
        {
            if (_size == 0)
            {
                item = null;
                return false;
            }

            item = _array[_size - 1];
            _size--;
            return true;
        }

        public virtual void Clear()
        {
            Array.Clear(_array, 0, _size);
            _size = 0;
        }

        public ArrayStack Clone()
        {
            var clone = new ArrayStack(_size);
            Array.Copy(_array, clone._array, _size);
            clone._size = _size;
            return clone;
        }

        IStack ICloneable<IStack>.Clone()
            => Clone();

        object ICloneable.Clone()
            => Clone();
        
        public ArrayStackEnumerator GetEnumerator()
            => new ArrayStackEnumerator(this);
        
        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public struct ArrayStackEnumerator : IEnumerator
        {
            private int _index;
            private object? _current;
            private readonly ArrayStack _stack;
            public ArrayStackEnumerator(ArrayStack stack)
            {
                _stack = stack;
                _index = -2;
                _current = null;
            }

            public bool MoveNext()
            {
                bool returnValue;
                switch (_index)
                {
                    case -2:
                    {
                        _index = _stack._size - 1;
                        returnValue = _index > 0;
                        if (returnValue)
                        {
                            _current = _stack._array[_index];
                        }

                        return returnValue;
                    }
                    case -1:
                        return false;
                }

                returnValue = --_index >= 0;
                _current = returnValue ? _stack._array[_index] : null;
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