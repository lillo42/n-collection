using System;
using System.Collections;
using System.Diagnostics;
using NCollection.DebugViews;

namespace NCollection
{
    [DebuggerTypeProxy(typeof(ICollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class LinkedStack : IStack, ICloneable<LinkedStack>
    {
        private Node? _current;

        public LinkedStack()
        {
            
        }

        public LinkedStack(IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            
            var item = enumerable.GetEnumerator();
            while (item.MoveNext())
            {
                Push(item.Current);
            }
        }
        
        public virtual int Count { get; private set; }

        public virtual bool IsSynchronized => false;

        public virtual object SyncRoot => this;

        public virtual bool IsReadOnly => false;
        
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
            if (_current == null)
            {
                item = null;
                return false;
            }

            item = _current.Value;
            return true;
        }

        public virtual void Push(object? item)
        {
            _current = new Node(_current, item);
            Count++;
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
            if (Count == 0 || _current == null)
            {
                item = null;
                return false;
            }

            item = _current.Value;
            _current = _current.Preview;
            Count--;
            return true;
        }

        public virtual void CopyTo(Array array, int index)
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
            
            var current = _current;
            for (var i = 0; current != null; i++)
            {
                source[i] = current.Value;
                current = current.Preview;
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
        
        public virtual bool Contains(object? item)
        {
            var node = _current;
            
            while (node != null)
            {
                if (item == null)
                {
                    if (node.Value == null)
                    {
                        return true;
                    }
                }
                else if (node.Value != null && node.Value.Equals(item))
                {
                    return true;
                }
                
                node = node.Preview;
            } 

            return false;
        }

        public LinkedStack Clone()
        {
           var clone =  new LinkedStack();
           
           var array = new object[Count];
           CopyTo(array, 0);

           for (var i = array.Length - 1 ; i >= 0; i--)
           {
               clone.Push(array[i]);
           }

           return clone;
        }

        public LinkedArrayEnumerator GetEnumerator()
            => new LinkedArrayEnumerator(this);
        
        IStack ICloneable<IStack>.Clone()
            => Clone();
        
        object ICloneable.Clone()
            => Clone();

        IEnumerator IEnumerable.GetEnumerator() 
            => new LinkedArrayEnumerator(this);

        private class Node
        {
            public Node(Node? preview, object? value)
            {
                Preview = preview;
                Value = value;
            }

            internal object? Value { get; }
            internal Node? Preview { get; }
        }

        public struct LinkedArrayEnumerator : IEnumerator
        {
            private readonly LinkedStack _stack;
            private Node? _current;

            public LinkedArrayEnumerator(LinkedStack stack)
            {
                _stack = stack;
                _current = _stack._current;
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
                _current = _current.Preview;
                return true;
            }

            public void Reset() 
                => _current = _stack._current;

            public object? Current { get; private set; }
        }
    }
}