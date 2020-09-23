﻿using System;
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
            
            public RedBlackNode? Parent { get; set; }
            public RedBlackNode? Right { get; set; }
            public RedBlackNode? Left { get; set; }

            public bool IsOnLeft => Parent?.Left == this;
            public bool HasRedChild => (Left != null && Left.Color == Color.Red) || (Right != null && Right.Color == Color.Red);
        }

        private int _version = int.MinValue;
        
        internal RedBlackNode? _root;

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
        {
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
            
            if (source is RedBlackTree<T> tree)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = tree.Count;
                Comparer = tree.Comparer;
                if(tree._root != null)
                {
                    _version++;
                    _root = new RedBlackNode();
                    CloneRecursive(tree._root, _root, tree._version, tree);
                }
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

            if (source is RedBlackTree<T> tree)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = tree.Count;
                if(tree._root != null)
                {
                    _version++;
                    _root = new RedBlackNode();
                    CloneRecursive(tree._root, _root, tree._version, tree);
                }
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
           _root = Insert(_root, item, Comparer);
           Count++;
           _version++;
           return true;
        }
        
        /// <inheritdoc />
        public override bool Remove(T item)
        {
            var node = Find(item);
            if (node == null)
            {
                return false;
            }
            
            _root = Delete(node);
            _version++;
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
            FreeRecursive(_root);
            _root = null;
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

                node.Left = null;
                node.Right = null;
                node.Parent = null;
                node.Value = default!;
            }
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

        private static RedBlackNode? GetGrandParent(RedBlackNode? node) => node?.Parent?.Parent;

        private static RedBlackNode? GetSibling(RedBlackNode? node)
        {
            var parent = node?.Parent;

            if (parent == null)
            {
                return null;
            }

            if (node == parent.Left)
            {
                return parent.Right;
            }
            else
            {
                return parent.Left;
            }
        }
        private static RedBlackNode? GetUncle(RedBlackNode? node) => GetSibling(node?.Parent);

        private static void RotateLeft(RedBlackNode node)
        {
            var right = node.Right!;
            var parent = node.Parent;

            node.Right = right.Left;
            right.Left = node;
            node.Parent = right;

            if (node.Right != null)
            {
                node.Right.Parent = node;
            }

            if (parent != null)
            {
                if (node == parent.Left)
                {
                    parent.Left = right;
                }
                else
                {
                    parent.Right = right;
                }
            }
            
            right.Parent = parent;
        }
        
        private static void RotateRight(RedBlackNode node)
        {
            var left = node.Left!;
            var parent = node.Parent;

            node.Left = left.Right;
            left.Right = node;
            node.Parent = left;

            if (node.Left != null)
            {
                node.Left.Parent = node;
            }

            
            if (parent != null)
            {
                if (node == parent.Left)
                {
                    parent.Left = left;
                }
                else
                {
                    parent.Right = left;
                }
            }
            
            left.Parent = parent;
        }

        private static RedBlackNode Insert(RedBlackNode? root, T item, [NotNull] IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            var node = new RedBlackNode
            {
                Value = item,
            };

            // Insert new Node into the current tree.
            InsertRecursive(root, node, comparer);
            
            // Repair the tree in case any of the red-black properties have been violated.
            FixInsert(node);

            // Find the new root to return.
            root = node;
            while (root.Parent != null)
            {
                root = root.Parent;
            }

            root.Color = Color.Black;
            return root;
            
            // Recursively descend the tree until a leaf is found.
            static void InsertRecursive(RedBlackNode? root, RedBlackNode node, IComparer<T> comparer)
            {
                var current = root;

                while (current != null)
                {
                    var cmp = comparer.Compare(node.Value, current.Value);
                    if (cmp < 0)
                    {
                        if (current.Left == null)
                        {
                            current.Left = node;
                            break;
                        }

                        current = current.Left;
                    }
                    else
                    {
                        if (current.Right == null)
                        {
                            current.Right = node;
                            break;
                        }

                        current = current.Right;
                    }
                }
                
                node.Parent = root;
                node.Left = null;
                node.Right = null;
                node.Color = Color.Red;
            }

            static void FixInsert(RedBlackNode? node)
            {
                while (node?.Parent != null && node.Color == Color.Red)
                {
                    var parent = node.Parent;
                    var grandParent = GetGrandParent(node);

                    if (grandParent == null)
                    {
                        break;
                    }

                    if (parent == grandParent.Left)
                    {
                        var uncle = grandParent.Right;
                        if (uncle != null && uncle.Color == Color.Red)
                        {
                            grandParent.Color = Color.Red;
                            parent.Color = Color.Black;
                            uncle.Color = Color.Black;
                            node = grandParent;
                        }
                        else
                        {
                            if (node == parent.Right)
                            {
                                RotateLeft(parent);
                                node = parent;
                                parent = parent.Parent;
                            }
                            
                            RotateRight(grandParent);
                            (parent!.Color, grandParent.Color) = (grandParent.Color, parent.Color);
                            node = parent;
                        }
                    }
                    else
                    {
                        var uncle = GetUncle(node);

                        if (uncle != null && uncle.Color == Color.Red)
                        {
                            grandParent.Color = Color.Red;
                            parent.Color = Color.Black;
                            uncle.Color = Color.Black;
                            node = grandParent;
                        }
                        else
                        {
                            if (node == parent.Left)
                            {
                                RotateRight(parent);
                                node = parent;
                                parent = parent.Parent;
                            }

                            RotateLeft(grandParent);
                            (parent!.Color, grandParent.Color) = (grandParent.Color, parent.Color);
                            node = parent;
                        }
                    }
                }
            }
        }

        private static RedBlackNode? Delete(RedBlackNode node)
        {
            var replace = Replace(node);
            var isBlack = (replace == null || replace.Color == Color.Black) && node.Color == Color.Black;

            var parent = node.Parent;

            if (replace == null) 
            {
                if (parent != null) 
                { 
                    if (isBlack) 
                    { 
                        FixDoubleBlack(node); 
                    } 
                    else
                    {
                        var sibling = GetSibling(node);
                        if (sibling != null)
                        {
                            sibling.Color = Color.Red;
                        }
                    }
                    
                    if (node == parent.Left)
                    {
                        parent.Left = null;
                    } 
                    else 
                    { 
                        parent.Right = null; 
                    } 
                }
                
                Free(node);
                return GetRoot(parent); 
            } 
            
            if (node.Left == null || node.Right == null) 
            { 
                if (node.Parent == null) 
                { 
                    node.Value = replace.Value; 
                    Free(node); 
                } 
                else 
                {
                    if (node.IsOnLeft) 
                    { 
                        parent!.Left = replace; 
                    } 
                    else 
                    { 
                        parent!.Right = replace; 
                    } 
                    
                    Free(node);
                    replace.Parent = parent; 
                    if (isBlack) 
                    { 
                        FixDoubleBlack(replace); 
                    } 
                    else 
                    { 
                        replace.Color = Color.Black; 
                    } 
                }
                return GetRoot(replace); 
            } 
            
            (node.Value, replace.Value) = (replace.Value, node.Value);
            return Delete(replace);

            static void Free(RedBlackNode node)
            {
                node.Parent = null;
                node.Left = null;
                node.Right = null;
                node.Value = default!;
            }
            
            static RedBlackNode? Replace(RedBlackNode node)
            {
                if (node.Left != null && node.Right != null)
                {
                    return Min(node.Right);
                }

                if (node.Left == null && node.Right != null)
                {
                    return null;
                }

                return node.Left ?? node.Right;
            }

            static RedBlackNode? Min(RedBlackNode node)
            {
                while (node.Left != null)
                {
                    node = node.Left;
                }

                return node;
            }

            static RedBlackNode? GetRoot(RedBlackNode? node)
            {
                if (node == null)
                {
                    return null;
                }
                
                while (node.Parent != null)
                {
                    node = node.Parent;
                }

                return node;
            }
            
            static void FixDoubleBlack(RedBlackNode node) 
            {
                if (node.Parent == null)
                {
                    // Reached root 
                    return;
                }

                var sibling = GetSibling(node);
                var parent = node.Parent;
                
                if (sibling == null) 
                { 
                    // No sibiling, double black pushed up 
                    FixDoubleBlack(parent); 
                }
                else 
                { 
                    if (sibling.Color == Color.Red) 
                    { 
                        // Sibling red 
                        parent.Color = Color.Red; 
                        sibling.Color = Color.Black;
                        if (sibling.IsOnLeft) 
                        { 
                            // left case 
                            RotateRight(parent); 
                        } 
                        else 
                        { 
                            // right case 
                            RotateLeft(parent); 
                        } 
                        
                        FixDoubleBlack(node); 
                    } 
                    else 
                    { 
                        // Sibling black 
                        if (sibling.HasRedChild) 
                        { 
                            // at least 1 red children 
                            if (sibling.Left != null && sibling.Left.Color == Color.Red) 
                            { 
                                if (sibling.IsOnLeft) 
                                { 
                                    // left left 
                                    sibling.Left.Color = sibling.Color; 
                                    sibling.Color = parent.Color; 
                                    RotateRight(parent); 
                                } 
                                else 
                                { 
                                    // right left 
                                    sibling.Left.Color = parent.Color; 
                                    RotateRight(sibling); 
                                    RotateLeft(parent); 
                                } 
                            } 
                            else 
                            { 
                                if (sibling.IsOnLeft) 
                                { 
                                    // left right 
                                    sibling.Right!.Color = parent.Color; 
                                    RotateLeft(sibling); 
                                    RotateRight(parent); 
                                } 
                                else 
                                { 
                                    // right right 
                                    sibling.Right!.Color = sibling.Color; 
                                    sibling.Color = parent.Color; 
                                    RotateLeft(parent); 
                                } 
                            } 
                            parent.Color = Color.Black; 
                        } 
                        else 
                        { 
                            // 2 black children 
                            sibling.Color = Color.Red;
                            if (parent.Color == Color.Black)
                            {
                                FixDoubleBlack(parent);
                            }
                            else
                            {
                                parent.Color = Color.Black;
                            } 
                        } 
                    } 
                } 
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
                    while (!_stack.IsEmpty || _node != null)
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

                if (_state == -2 || _node == null)
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