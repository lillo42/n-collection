using System;
using System.Collections.Generic;
using System.Collections;

namespace NCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HashSet<T> : AbstractSet<T>, ICloneable
    {
        private int _version = int.MinValue;
        private RedBlackTree<T>.RedBlackNode?[] _elements;

        /// <summary>
        /// Initialize <see cref="HashSet{T}"/>.
        /// </summary>
        public HashSet()
        {
            _elements = new RedBlackTree<T>.RedBlackNode[16];
        }
        
        /// <summary>
        /// Initialize <see cref="HashSet{T}"/>.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the array</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0</exception>
        public HashSet(int initialCapacity)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The value should be greater or equal than 1");
            }
            
            _elements = new RedBlackTree<T>.RedBlackNode[initialCapacity];
        }
        
        /// <summary>
        /// Initialize <see cref="HashSet{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this set.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="comparer"/> is <see langword="null"/>.</exception>
        public HashSet(IComparer<T> comparer)
            : base(comparer)
        {
            _elements = _elements = new RedBlackTree<T>.RedBlackNode[16];
        }

        /// <summary>
        /// Initialize <see cref="HashSet{T}"/>.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the array</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this set.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="comparer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0</exception>
        public HashSet(int initialCapacity, IComparer<T> comparer)
            : this(comparer)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The value should be greater or equal than 1");
            }
            
            _elements = new RedBlackTree<T>.RedBlackNode[initialCapacity];
        }

        /// <summary>
        /// Initialize <see cref="HashSet{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> is null </exception>
        public HashSet(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _elements = new RedBlackTree<T>.RedBlackNode[16];

            if (source is System.Collections.Generic.ICollection<T> collection)
            {
                if (collection.Count > 0)
                {
                    _elements = new RedBlackTree<T>.RedBlackNode?[collection.Count];
                }

                if (source is AbstractSet<T> set)
                {
                    // ReSharper disable once VirtualMemberCallInConstructor
                    Comparer = set.Comparer;
                }
            }

            foreach (var item in source)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                TryAdd(item);
            }
        }
        
        /// <summary>
        /// Initialize <see cref="HashSet{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this set.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> is null </exception>
        public HashSet(IEnumerable<T> source, IComparer<T> comparer)
            : this(comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            _elements = new RedBlackTree<T>.RedBlackNode?[16];
            if (source is System.Collections.Generic.ICollection<T> collection)
            {
                if (collection.Count > 0)
                {
                    _elements = new RedBlackTree<T>.RedBlackNode?[collection.Count];
                }
            }

            foreach (var item in source)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                TryAdd(item);
            }
        }

        private int Index(int hash)
        {
            if (hash == 0)
            {
                return 0;
            }
            
            return hash % _elements.Length;
        }

        /// <inheritdoc />
        public override bool TryAdd(T item)
        {
            while (true)
            {
                var hash = Hash(item);

                var index = Index(hash);
                if (_elements[index] == null)
                {
                    _elements[index] = new RedBlackTree<T>.RedBlackNode(item);
                }
                else if (Hash(_elements[index]!.Value) == hash)
                {
                    if (!RedBlackTree<T>.Insert(ref _elements[index], item, Comparer, RedBlackTree<T>.InsertBehavior.NotInsertIfExist))
                    {
                        return false;
                    }
                }
                else
                {
                    var tmp = new RedBlackTree<T>.RedBlackNode[_elements.Length * 2];

                    foreach (var value in _elements)
                    {
                        if (value != null)
                        {
                            index = Hash(value!.Value);
                            tmp[index % tmp.Length] = value;
                        }
                    }

                    _elements = tmp;
                    continue;
                }

                Count++;
                return true;
            }
        }

        /// <inheritdoc />
        public override bool Remove(T item)
        {
            var hash = Hash(item);
            var index = Index(hash);
            if (_elements[index] == null)
            {
                return false;
            }

            var result = RedBlackTree<T>.Delete(ref _elements[index], _elements[index], item, Comparer);
            if (result)
            {
                Count--;
            }

            return result;
        }

        /// <inheritdoc />
        public override bool Contains(T item)
        {
            if (Count == 0)
            {
                return false;
            }

            var index = Index(Hash(item));

            return RedBlackTree<T>.Find(_elements[index], item, Comparer) != null;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            Count = 0;
            Array.Clear(_elements, 0, _elements.Length);
        }

        /// <summary>
        /// Creates a new <see cref="HashSet{T}"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="HashSet{T}"/> that is a copy of this instance.</returns>
        public HashSet<T> Clone()
        {
            return new HashSet<T>(this);
        }

        object ICloneable.Clone()
            => Clone();
        
        /// <inheritdoc />
        public override IEnumerator<T> GetEnumerator()
        {
            return new HashSetEnumerator(this, _version);
        }
        
        private struct HashSetEnumerator : IEnumerator<T>
        {
            private readonly HashSet<T> _hash;
            private readonly int _version;
            private IEnumerator<T>? _current;
            private int _index;

            public HashSetEnumerator(HashSet<T> hash, int version)
            {
                _hash = hash;
                _version = version;
                _index = 0;
                _current = null;
            }

            public bool MoveNext()
            {
                if (_version != _hash._version)
                {
                    throw new InvalidOperationException("RedBlackTree was changed");
                }
                
                if (_current != null && _current.MoveNext())
                {
                    return true;
                }

                while (_index < _hash._elements.Length)
                {
                    var item = _hash._elements[_index++]; 
                    if (item != null)
                    {
                        _current = new RedBlackTree<T>.RedBlackTreeInOrderTraversalEnumerator(item);
                        _current.MoveNext();
                        return true;
                    }
                }

                _current = null;
                return false;
            }

            public void Reset()
            {
                _index = 0;
                _current = null;
            }

            public T Current
            {
                get
                {
                    if (_current == null)
                    {
                        return default!;
                    }

                    return _current.Current;
                }
            }

            object? IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }
        }
    }
}
