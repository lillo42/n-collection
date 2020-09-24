using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace NCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
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

            public Color Color { get; set; }

            public RedBlackNode? Parent { get; set; } = default!;
            public RedBlackNode Right { get; set; } = default!;
            public RedBlackNode Left { get; set; } = default!;
        }

        private int _version = int.MinValue;
        
        private RedBlackNode _root;
        private static readonly RedBlackNode TNULL = new RedBlackNode
        {
            Color = Color.Black
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{T}"/>.
        /// </summary>
        public RedBlackTree()
        {
            Comparer = Comparer<T>.Default;
            _root = TNULL;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this red-black tree.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/></exception>
        public RedBlackTree(IComparer<T> comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _root = TNULL;
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
            
            _root = TNULL;
            
            if (source is RedBlackTree<T> tree)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = tree.Count;
                Comparer = tree.Comparer;
                _version++;
                CloneRecursive(tree._root, _root, tree._version, tree);
            }
            else
            {
                foreach (var item in source)
                {
                    // ReSharper disable once VirtualMemberCallInConstructor
                    TryAdd(item);
                }
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
            
            _root = TNULL;

            if (source is RedBlackTree<T> tree)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = tree.Count;
                _version++;
                CloneRecursive(tree._root, _root, tree._version, tree);
            }
            else
            {
                foreach (var item in source)
                {
                    // ReSharper disable once VirtualMemberCallInConstructor
                    TryAdd(item);
                }
            }
        }
        
        private static void CloneRecursive(RedBlackNode original, RedBlackNode @new, in int version, in RedBlackTree<T> tree)
        {
            if (version != tree._version)
            {
                throw new InvalidOperationException("RedBlackTree was changed");
            }
                
            @new.Value = original.Value;

            if (original.Left != null)
            {
                @new.Left = new RedBlackNode();
                CloneRecursive(original.Left, @new.Left, version, tree);
            }
                
            if (original.Right != null)
            {
                @new.Right = new RedBlackNode();
                CloneRecursive(original.Right, @new.Right, version, tree);
            }
        }

        /// <summary>
        /// The <see cref="IComparer{T}"/>
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <inheritdoc />
        public override bool TryAdd(T item)
        {
            Insert(ref _root, item, Comparer);
            Count++;
            _version++;
            return true;
        }
        
        /// <inheritdoc />
        public override bool Remove(T item)
        {
            if (!Delete(ref _root, _root, item, Comparer))
            {
                return false;
            }
            
            Count--;
            _version++;
            return true;
        }

        /// <inheritdoc />
        public override bool Contains(T item)
        {
            return Find(item) != TNULL;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            FreeRecursive(_root);
            _root = TNULL;
            Count = 0;
            _version = int.MinValue;
            
            static void FreeRecursive(RedBlackNode? node)
            {
                if (node == null)
                {
                    return;
                }
                
                FreeRecursive(node.Left);
                FreeRecursive(node.Right);

                node.Left = null!;
                node.Right = null!;
                node.Parent = null;
                node.Value = default!;
            }
        }

        private RedBlackNode Find(T item)
        {
            var current = _root;
            while (current != TNULL)
            {
                var result = Comparer.Compare(item, current.Value);
                if (result == 0)
                {
                    return current;
                }

                current = result > 0 ? current.Right : current.Left;
            }
            
            return TNULL;
        }
        
        private static void Insert(ref RedBlackNode root, in T item, [NotNull] in IComparer<T> comparer) 
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            
            var node = new RedBlackNode
            {
                Parent = null,
                Value = item,
                Left = TNULL,
                Right = TNULL,
                Color = Color.Red
            };

            RedBlackNode? other = null;
            var current = root;

            while (current != TNULL) 
            {
                other = current;
                if (comparer.Compare(node.Value, current.Value) < 0) 
                {
                    current = current.Left!;
                } 
                else 
                {
                    current = current.Right!;
                }
            }

            node.Parent = other;
            if (other == null) 
            {
                root = node;
            } 
            else if (comparer.Compare(node.Value, other.Value) < 0) 
            {
                other.Left = node;
            } 
            else 
            {
                other.Right = node;
            }

            if (node.Parent == null) 
            {
                node.Color = Color.Black;
                return;
            }

            if (node.Parent.Parent == null) 
            {
                return;
            }

            FixInsert(ref root, node);
            
            static void FixInsert(ref RedBlackNode root, RedBlackNode current)
            {
                while (current.Parent!.Color == Color.Red) 
                {
                    RedBlackNode uncle;
                    if (current.Parent == current.Parent.Parent!.Right) 
                    {
                        uncle = current.Parent.Parent.Left!;
                        if (uncle.Color == Color.Red) 
                        {
                            uncle.Color = Color.Black;
                            current.Parent.Color = Color.Black;
                            current.Parent.Parent.Color = Color.Red;
                            current = current.Parent.Parent;
                        } 
                        else 
                        {
                            if (current == current.Parent.Left) 
                            {
                                current = current.Parent;
                                RightRotate(ref root, current);
                            }
                            current.Parent!.Color = Color.Black;
                            current.Parent.Parent!.Color = Color.Red;
                            LeftRotate(ref root, current.Parent.Parent);
                        }
                    } 
                    else 
                    {
                        uncle = current.Parent.Parent.Right!;

                        if (uncle.Color == Color.Red) 
                        {
                            uncle.Color = Color.Black;
                            current.Parent.Color = Color.Black;
                            current.Parent.Parent.Color = Color.Red;
                            current = current.Parent.Parent;
                        } 
                        else 
                        {
                            if (current == current.Parent.Right) 
                            {
                                current = current.Parent;
                                LeftRotate(ref root, current);
                            }

                            current.Parent!.Color = Color.Black;
                            current.Parent.Parent!.Color = Color.Red;
                            RightRotate(ref root, current.Parent.Parent);
                        }
                    }
            
                    if (current == root) 
                    {
                        break;
                    }
                }
        
                root.Color = Color.Black;
            }
        }

        private static void LeftRotate(ref RedBlackNode root, RedBlackNode node) 
        {
            var right = node.Right!;
            node.Right = right.Left;
            if (right.Left != TNULL) 
            {
                right.Left.Parent = node;
            }
            
            right.Parent = node.Parent;
            if (node.Parent == null) 
            {
                root = right;
            } 
            else if (node == node.Parent.Left) 
            {
                node.Parent.Left = right;
            } 
            else 
            {
                node.Parent.Right = right;
            }
            
            right.Left = node;
            node.Parent = right;
        }

        private static void RightRotate(ref RedBlackNode root, RedBlackNode node)
        {
            var left = node.Left!;
            node.Left = left.Right;
            if (left.Right != TNULL)
            {
                left.Right.Parent = node;
            }

            left.Parent = node.Parent;
            if (node.Parent == null)
            {
                root = left;
            }
            else if (node == node.Parent.Right)
            {
                node.Parent.Right = left;
            }
            else
            {
                node.Parent.Left = left;
            }

            left.Right = node;
            node.Parent = left;
        }
        
        // Balance the tree after deletion of a node
        private static bool Delete(ref RedBlackNode root, RedBlackNode node, in T value, [NotNull] in IComparer<T> comparer) 
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            
            var current = TNULL;
           
            while (node != TNULL)
            {
                if (comparer.Compare(node.Value, value) == 0) 
                {
                    current = node;
                }
                
                node = comparer.Compare(node.Value, value) <= 0 ? node.Right : node.Left;
            }

            if (current == TNULL) 
            {
                return false;
            }

            var yOriginalColor = current.Color;
            
            RedBlackNode toFix;
            if (current.Left == TNULL) 
            {
                toFix = current.Right;
                RbTransplant(ref root, current, current.Right);
            } 
            else if (current.Right == TNULL) 
            {
                toFix = current.Left;
                RbTransplant(ref root, current, current.Left);
            } 
            else 
            {
                var minimum = Minimum(current.Right);
                yOriginalColor = minimum.Color;
                toFix = minimum.Right;
                if (minimum.Parent == current) 
                {
                    toFix.Parent = minimum;
                } 
                else 
                {
                    RbTransplant(ref root, minimum, minimum.Right);
                    minimum.Right = current.Right;
                    minimum.Right.Parent = minimum;
                }

                RbTransplant(ref root, current, minimum);
                minimum.Left = current.Left;
                minimum.Left.Parent = minimum;
                minimum.Color = current.Color;
            }
            
            if (yOriginalColor == Color.Black) 
            {
                FixDelete(ref root, toFix);
            }

            return true;
            
            static RedBlackNode Minimum(RedBlackNode node) 
            {
                while (node.Left != TNULL) {
                    node = node.Left;
                }
                return node;
            }
            
            static void RbTransplant(ref RedBlackNode root, RedBlackNode node, RedBlackNode other) 
            {
                if (node.Parent == null) 
                {
                    root = other;
                } 
                else if (node == node.Parent.Left) 
                {
                    node.Parent.Left = other;
                } 
                else 
                {
                    node.Parent.Right = other;
                }
            
                other.Parent = node.Parent;
            }
            
            static void FixDelete(ref RedBlackNode root, RedBlackNode node) 
            {
                while (node != root && node.Color == Color.Black)
                {
                    RedBlackNode sibling;
                    if (node == node.Parent.Left) 
                    {
                        sibling = node.Parent.Right;
                        if (sibling.Color == Color.Red) 
                        {
                            sibling.Color = Color.Black;
                            node.Parent.Color = Color.Red;
                            LeftRotate(ref root, node.Parent);
                            sibling = node.Parent.Right;
                        }

                        if (sibling.Left.Color == Color.Black && sibling.Right.Color == Color.Black) 
                        {
                            sibling.Color = Color.Red;
                            node = node.Parent;
                        } 
                        else 
                        {
                            if (sibling.Right.Color == Color.Black) 
                            {
                                sibling.Left.Color = Color.Black;
                                sibling.Color = Color.Red;
                                RightRotate(ref root, sibling);
                                sibling = node.Parent.Right;
                            }

                            sibling.Color = node.Parent.Color;
                            node.Parent.Color = Color.Black;
                            sibling.Right.Color = Color.Black;
                            LeftRotate(ref root, node.Parent);
                            node = root;
                        }
                    } 
                    else 
                    {
                        sibling = node.Parent.Left;
                        if (sibling.Color == Color.Red) 
                        {
                            sibling.Color = Color.Black;
                            node.Parent.Color = Color.Red;
                            RightRotate(ref root, node.Parent);
                            sibling = node.Parent.Left;
                        }

                        if (sibling.Right.Color == Color.Black && sibling.Right.Color == Color.Black) 
                        {
                            sibling.Color = Color.Red;
                            node = node.Parent;
                        } 
                        else 
                        {
                            if (sibling.Left.Color == Color.Black) 
                            {
                                sibling.Right.Color = Color.Black;
                                sibling.Color = Color.Red;
                                LeftRotate(ref root, sibling);
                                sibling = node.Parent.Left;
                            }

                            sibling.Color = node.Parent.Color;
                            node.Parent.Color = Color.Black;
                            sibling.Left.Color = Color.Black;
                            RightRotate(ref root, node.Parent);
                            node = root;
                        }
                    }
                }
            
                node.Color = Color.Black;
        }
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
            private readonly int _version;
            private RedBlackNode? _node;
            private int _state;

            public InorderTraversalEnumerator(RedBlackTree<T> tree)
            {
                _tree = tree;
                _version = _tree._version;
                _node = null;
                _state = -1;
                _stack =new LinkedStack<RedBlackNode>();
            }

            public bool MoveNext()
            {
                if (_version != _tree._version)
                {
                    throw new InvalidOperationException("RedBlackTree was changed");
                }

                if (_state == -1)
                {
                    _node = _tree._root;
                    _state = 0;
                }
                else if (_state == 0)
                {
                    _node = _node?.Right;
                }
               
                if(_state != -2)
                {
                    while (!_stack.IsEmpty || _node != null || _node != TNULL)
                    {
                        if (_node != null)
                        {
                            _stack.Push(_node);
                            _node = _node.Left;
                        }
                        else
                        {
                            _node = _stack.Pop();
                            break;
                        }
                    }
                }

                if (_state == -2 || _node == null || _node == TNULL)
                {
                    _state = -2;
                    _node = null;
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                _state = -1;
                _node = null;
                _stack.Clear();
            }

            public T Current => _node == null ? default! : _node.Value;

            object? IEnumerator.Current => Current;

            public void Dispose()
            {
                _stack.Clear();
            }
        }
       
        /// <inheritdoc />
        public override IEnumerator<T> PreorderTraversal()
        {
            return new PreorderTraversalEnumerator(this);
        }
        
        private struct PreorderTraversalEnumerator : IEnumerator<T>
        {
            private readonly RedBlackTree<T> _tree;
            private readonly LinkedStack<RedBlackNode> _stack;
            private readonly int _version;
            private RedBlackNode? _node;
            private int _state;

            public PreorderTraversalEnumerator(RedBlackTree<T> tree)
            {
                _tree = tree;
                _version = _tree._version;
                _node = null;
                _state = -1;
                _stack =new LinkedStack<RedBlackNode>();
            }

            public bool MoveNext()
            {
                if (_version != _tree._version)
                {
                    throw new InvalidOperationException("RedBlackTree was changed");
                }
                
                if (_state == -1)
                {
                    _node = _tree._root;
                    _state = 0;
                }
                else if (_state == 0)
                {
                    if (_node!.Left != null)
                    {
                        _stack.Push(_node);
                        _node = _node.Left;
                    }
                    else if (_node.Right != null)
                    {
                        _stack.Push(_node);
                        _node = _node.Right;
                    }
                    else
                    {
                        var current = _stack.Pop();
                        while (current.Right == null && !_stack.IsEmpty)
                        {
                            current = _stack.Pop();
                        }
                        
                        _node = current.Right;
                    }
                }

                if (_state == -2 || _node == null)
                {
                    _state = -2;
                    return false;
                }
                
                return true;
            }

            public void Reset()
            {
                _state = -1;
                _node = null;
                _stack.Clear();
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

            public void Dispose()
            {
                _stack.Clear();
            }
        }

        /// <inheritdoc />
        public override IEnumerator<T> PostorderTraversal()
        {
            return new PostorderTraversalEnumerator(this);
        }
        
        private struct PostorderTraversalEnumerator : IEnumerator<T>
        {
            private readonly RedBlackTree<T> _tree;
            private readonly LinkedStack<RedBlackNode> _stack;
            private readonly int _version;
            private RedBlackNode? _node;
            private int _state;

            public PostorderTraversalEnumerator(RedBlackTree<T> tree)
            {
                _tree = tree;
                _version = _tree._version;
                _node = null;
                _state = -1;
                _stack =new LinkedStack<RedBlackNode>();
            }

            public bool MoveNext()
            {
                if (_version != _tree._version)
                {
                    throw new InvalidOperationException("RedBlackTree was changed");
                }
                
                if (_state == -1)
                {
                    _node = _tree._root;
                    _state = 0;
                }
                
                if (_state != -2)
                {
                    var node = _node;
                    var last = node;
                    
                    if (_state == 0)
                    {
                        node = _stack.Pop();
                    }
                    
                    while (!_stack.IsEmpty || node != null)
                    {
                        if (node != null)
                        {
                            if (node.Right == last)
                            {
                                _node = node;
                                break;
                            }

                            _stack.Push(node);
                            if (node.Left == last)
                            {
                                last = node;
                                node = node.Right;
                            }
                            else
                            {
                                last = node;
                                node = node.Left;
                            }
                        }
                        else
                        {
                            last = node;
                            node = _stack.Pop();
                        }
                    }
                }



                if (_state == -2 || _node == null)
                {
                    _state = -2;
                    return false;
                }
                
                return true;
            }

            public void Reset()
            {
                _state = -1;
                _node = null;
                _stack.Clear();
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

            public void Dispose()
            {
                _stack.Clear();
            }
        }
    }
}