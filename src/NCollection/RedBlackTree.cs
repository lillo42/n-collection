using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedBlackTree<T> : AbstractTree<T>, ICloneable
    {
        internal enum Color : byte
        {
            Black,
            Red
        }

        internal class RedBlackNode
        {
            public T Value { get; set; } = default!;
            public Color Color { get; set; } = Color.Black;
            public RedBlackNode? Parent { get; set; }
            public RedBlackNode? Right { get; set; }
            public RedBlackNode? Left { get; set; }
        }
        
        private RedBlackNode? _root;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{T}"/>.
        /// </summary>
        public RedBlackTree()
        {
            Comparer = Comparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this red-black tree.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/></exception>
        public RedBlackTree(IComparer<T> comparer)
        {;
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null</exception>
        public RedBlackTree(IEnumerable<T> source)
            : this()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var item in source)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                TryAdd(item);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue{T}"/>.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to copy elements from.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this priority queue</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="comparer"/> is <see langword="null"/></exception>
        public RedBlackTree(IComparer<T> comparer, IEnumerable<T> source)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
           
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var item in source)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                TryAdd(item);
            }
        }

        /// <summary>
        /// The <see cref="IComparer{T}"/>
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <inheritdoc />
        public override bool TryAdd(T item)
        {
           _root = Insert(_root, item, Comparer);
           _root.Color = Color.Black;
           Count++; 
           return true;
        }
        
        /// <inheritdoc />
        public override bool Remove(T item)
        {
            var node = Delete(_root, item, Comparer);
            if (node == null)
            {
                return false;
            }

            _root = node;
            return true;
        }

        /// <inheritdoc />
        public override bool Contains(T item)
        {
            return Find(item) != null;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            _root = null;
            Count = 0;
        }

        private RedBlackNode? Find(T item)
        {
            var current = _root;
            while (current != null)
            {
                var result = Comparer.Compare(item, current.Value);
                if (result == 0)
                {
                    return current;
                }
                else if (result > 0)
                {
                    current = current.Right;
                }
                else
                {
                    current = current.Left;
                }
            }
            
            return null;
        }

        internal static RedBlackNode Insert(RedBlackNode? root, T item, IComparer<T> comparer)
        {
            if (root == null)
            {
                return new RedBlackNode
                {
                    Value = item,
                    Color = Color.Red
                };
            }
            
            var cmp = comparer.Compare(item, root.Value);
            if (cmp < 0)
            {
                root.Left = Insert(root.Left, item, comparer);
            }
            else
            {
                root.Right = Insert(root.Right, item, comparer);
            }

            if (IsRed(root.Right) && !IsRed(root.Left))
            {
                root = RotateLeft(root);
            }

            if (IsRed(root.Left) && IsRed(root.Left!.Left))
            {
                
                root = RotateRight(root);
            }

            if (IsRed(root.Right) && IsRed(root.Left))
            {
                FlipColors(root);
            }
            
            return root;
        }

        internal static RedBlackNode? Delete(RedBlackNode? root, T item, [NotNull] IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            
            if (root == null)
            {
                return null;
            }


            if (!IsRed(root.Left) && !IsRed(root.Right))
            {
                root.Color = Color.Red;
            }

            root = DeleteHelper(root, item, comparer);
            if (root != null)
            {
                root.Color = Color.Black;
            }

            return root;

            static RedBlackNode? DeleteHelper(RedBlackNode node, T item, IComparer<T> comparer)
            {
                if (comparer.Compare(item, node.Value) < 0)
                {
                    if (!IsRed(node.Left) && !IsRed(node.Left!.Left))
                    {
                        node = MoveRedLeft(node);
                    }

                    node.Left = DeleteHelper(node.Left!, item, comparer);
                }
                else
                {
                    if (IsRed(node.Left))
                    {
                        node = RotateRight(node);
                    }

                    if (comparer.Compare(item, node.Value) == 0 && node.Right == null)
                    {
                        return null;
                    }

                    if (!IsRed(node.Right) && !IsRed(node.Right!.Left))
                    {
                        node = MoveRedRight(node);
                    }

                    if (comparer.Compare(item, node.Value) == 0)
                    {
                        var other = Min(node.Right);
                        node.Value = other.Value;
                        node.Right = DeleteMin(node.Right);
                    }
                    else
                    {
                        node.Right = DeleteHelper(node.Right!, item, comparer);
                    }
                }

                return Balance(node);
            }

            // Assuming that h is red and both h.left and h.left.left
            // are black, make h.left or one of its children red.
            static RedBlackNode MoveRedLeft(RedBlackNode node)
            {
                FlipColors(node);
                if (IsRed(node.Right!.Left))
                {
                    node.Right = RotateRight(node.Right);
                    node = RotateLeft(node);
                    FlipColors(node);
                }

                return node;
            }

            // Assuming that h is red and both h.right and h.right.left
            // are black, make h.right or one of its children red.
            static RedBlackNode MoveRedRight(RedBlackNode node)
            {
                // assert (h != null);
                // assert isRed(h) && !isRed(h.right) && !isRed(h.right.left);
                FlipColors(node);
                if (IsRed(node.Left!.Left))
                {
                    node = RotateRight(node);
                    FlipColors(node);
                }

                return node;
            }

            // delete the key-value pair with the minimum key rooted at h
            static RedBlackNode? DeleteMin(RedBlackNode? node)
            {
                if (node!.Left == null)
                {
                    return null;
                }

                if (!IsRed(node.Left) && !IsRed(node.Left!.Left))
                {
                    node = MoveRedLeft(node);
                }

                node.Left = DeleteMin(node.Left);
                return Balance(node);
            }

            static RedBlackNode Balance(RedBlackNode node)
            {
                if (IsRed(node.Right))
                {
                    node = RotateLeft(node);
                }

                if (IsRed(node.Left) && IsRed(node.Left!.Left))
                {
                    node = RotateRight(node);
                }

                if (IsRed(node.Left) && IsRed(node.Right))
                {
                    FlipColors(node);
                }

                return node;
            }
        }

        // make a left-leaning link lean to the right
        private static RedBlackNode RotateRight(RedBlackNode redBlackNode) 
        {
            var other = redBlackNode.Left!;
            redBlackNode.Left = other.Right;
            other.Right = redBlackNode;
            other.Color = other.Right.Color;
            other.Right.Color = Color.Red;
            
            return other;
        }

         // the smallest key in subtree rooted at x; null if no such key
        private static RedBlackNode? Min(RedBlackNode? node)
        {
            if (node == null)
            {
                return null;
            }
            
            var current = node!;
            while (current.Left != null)
            {
                current = current.Left;
            }

            return current;
        }
        // make a right-leaning link lean to the left
        private static RedBlackNode RotateLeft(RedBlackNode redBlackNode) 
        {
            var other = redBlackNode.Right!;
            redBlackNode.Right = other.Left;
            other.Left = redBlackNode;
            other.Color = other.Left.Color;
            other.Left.Color = Color.Red;
            
            return other;
        }

        // flip the colors of a node and its two children
        private static void FlipColors(RedBlackNode redBlackNode) 
        {
            // h must have opposite color of its two children
            // assert (h != null) && (h.left != null) && (h.right != null);
            // assert (!isRed(h) &&  isRed(h.left) &&  isRed(h.right))
            //    || (isRed(h)  && !isRed(h.left) && !isRed(h.right));
            redBlackNode.Color = Invert(redBlackNode.Color);
            redBlackNode.Left!.Color = Invert(redBlackNode.Left.Color);
            redBlackNode.Right!.Color = Invert(redBlackNode.Right.Color);

            static Color Invert(Color color)
            {
                return color switch
                {
                    Color.Black => Color.Red,
                    Color.Red => Color.Black,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private static bool IsRed(RedBlackNode? node)
        {
            if (node == null)
            {
                return false;
            }
            
            return node.Color == Color.Red;
        }
        
        /// <summary>
        /// Creates a shallow copy of the <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="RedBlackTree{T}"/>.</returns>
        public RedBlackTree<T> Clone() => new RedBlackTree<T>(this);
        object ICloneable.Clone() => Clone();
        
        /// <inheritdoc />
        public override IEnumerator<T> InorderTraversal()
        {
            return new InorderTraversalEnumerator(this);
        }
        
        
        private struct InorderTraversalEnumerator : IEnumerator<T>
        {
            private readonly RedBlackTree<T> _tree;
            private readonly LinkedStack<RedBlackNode> _stack;
            private RedBlackNode? _node;
            private int _state;

            public InorderTraversalEnumerator(RedBlackTree<T> tree)
            {
                _tree = tree;
                _node = null;
                _state = -1;
                _stack =new LinkedStack<RedBlackNode>();
            }

            public bool MoveNext()
            {
                if (_state == -1)
                {
                    _node = Min(_tree._root, _stack);
                    _state = 0;
                }
                else if (_state == 0)
                {
                    _stack.TryPop(out _node);
                    if (_node?.Right != null)
                    {
                        _state = 1;
                    }
                }
                else if(_state == 1)
                {
                    _node = Min(_node!.Right, _stack);
                    _state = 0;
                }

                if (_state == -2 || _node == null)
                {
                    _state = -2;
                    return false;
                }

                return true;

                static RedBlackNode? Min(RedBlackNode? node, LinkedStack<RedBlackNode> stack)
                {
                    if (node == null)
                    {
                        return null;
                    }

                    var current = node;
                    while (current.Left != null)
                    {
                        stack.Push(current);
                        current = current.Left;
                    }

                    return current;
                }
            }

            public void Reset()
            {
                _state = -1;
            }

            public T Current
            {
                get
                {
                    if (_node == null)
                    {
                        return default!;
                    }

                    return _node.Value;
                }
            }

            object? IEnumerator.Current => Current;

            public void Dispose() { }
        }
        /// <inheritdoc />
        public override IEnumerator<T> PreorderTraversal()
        {
            throw new NotImplementedException();
        }
        

        /// <inheritdoc />
        public override IEnumerator<T> PostorderTraversal()
        {
            throw new NotImplementedException();
        }
    }
}