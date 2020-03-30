using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NCollection.Generics.DebugViews;

namespace NCollection.Generics
{
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class LinkedStack<T> : IStack<T>, IStack, ICloneable<LinkedStack<T>>
    {
        private Node? _current;
        public LinkedStack()
        {
            
        }

        public LinkedStack(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            
            
            foreach (var item in enumerable)
            {
                Push(item);
            }
        }
        
        public virtual int Count { get; private set; }

        public virtual bool IsSynchronized => false;

        public virtual object SyncRoot => this;

        public virtual bool IsReadOnly => false;
        

        [return: MaybeNull]
        public virtual T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        public virtual bool TryPeek([MaybeNullWhen(false)]out T item)
        {
            if (_current == null)
            {
                item = default!;
                return false;
            }

            item = _current.Value;
            return true;
        }

        public void Push(T item)
        {
            _current = new Node(_current, item);
            Count++;
        }
        
        [return: MaybeNull]
        public virtual T Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }
        
        public virtual bool TryPop([MaybeNullWhen(false)]out T item)
        {
            if (Count == 0 || _current == null)
            {
                item = default!;
                return false;
            }

            item = _current.Value;
            _current = _current.Preview;
            Count--;
            return true;
        }
        
        public virtual bool Contains(T item)
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
                else if (node.Value != null && EqualityComparer<T>.Default.Equals(node.Value, item))
                {
                    return true;
                }
                
                node = node.Preview;
            } 

            return false;
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

            
            var source = new T[Count];
            
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

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Index out  of range");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Invalid Off length");
            }
            
            var current = _current;
            for (var i = 0; current != null; i++)
            {
                array[i] = current.Value;
                current = current.Preview;
            }
        }

        public LinkedStackEnumerator GetEnumerator()
            => new LinkedStackEnumerator(this);
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public LinkedStack<T> Clone() 
        {
            var clone =  new LinkedStack<T>();
           
            var array = new T[Count];
            CopyTo(array, 0);

            for (var i = array.Length - 1 ; i >= 0; i--)
            {
                clone.Push(array[i]);
            }

            return clone;
        }

        IStack<T> ICloneable<IStack<T>>.Clone()
            => Clone();

        IStack ICloneable<IStack>.Clone() 
            => Clone();
        
        object ICloneable.Clone()
            => Clone();
        
        void IStack.Push(object? item)
            => Push((T) item!);

        bool IStack.TryPop(out object? item)
        {
            if (TryPop(out var result))
            {
                item = result;
                return true;
            }

            item = null;
            return false;
        }
        
        bool ICollection.Contains(object? item)
            => Contains((T) item!);

        bool IStack.TryPeek(out object? item)
        {
            if (TryPeek(out var result))
            {
                item = result;
                return true;
            }

            item = null;
            return false;
        }
        
        private class Node
        {
            public Node(Node? preview, T value)
            {
                Preview = preview;
                Value = value;
            }

            internal T Value { get; }
            internal Node? Preview { get; }
        }

        public struct LinkedStackEnumerator : IEnumerator<T>
        {
            private readonly LinkedStack<T> _stack;
            private Node? _current;

            public LinkedStackEnumerator(LinkedStack<T> stack)
            {
                _stack = stack;
                _current = _stack._current;
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
                _current = _current.Preview;
                return true;
            }

            public void Reset() 
                => _current = _stack._current;

            object? IEnumerator.Current => Current;

            public T Current { get; private set; }
            
            public void Dispose()
            {
            }
        }
    }
}