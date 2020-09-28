using System;
using System.Buffers;
using System.Collections.Generic;

namespace NCollection
{
    using JetBrains.Annotations;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HashSet<T> : AbstractSet<T>
    {
        
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
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The value should be greater or equal than 1");
            }
            
            _elements = new RedBlackTree<T>.RedBlackNode[initialCapacity];
        }
        
        public override IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool TryAdd(T item)
        {
            var hash = Hash(item);

            var index = hash % _elements.Length;
            if (_elements[index] == null)
            {
                _elements[index] = new RedBlackTree<T>.RedBlackNode(item);
            }
            else if (Hash(_elements[index]!.Value) == hash)
            {
                if (!RedBlackTree<T>.Insert(ref _elements[index], item, Comparer,
                    RedBlackTree<T>.InsertBehavior.NotInsertIfExist))
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
                index = hash % _elements.Length;
                _elements[index] = new RedBlackTree<T>.RedBlackNode(item);
            }

            Count++;
            return true;
        }

        /// <inheritdoc />
        public override bool Remove(T item)
        {
            var hash = Hash(item);
            var index = hash % _elements.Length;
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
    }
}
