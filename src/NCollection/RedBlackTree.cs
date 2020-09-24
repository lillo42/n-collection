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
        // internal enum Color : byte
        // {
        //     Black,
        //     Red
        // }

        internal class RedBlackNode
        {
            public T Value { get; set; } = default!;

            public int color { get; set; }
            
            public RedBlackNode? parent { get; set; }
            public RedBlackNode? right { get; set; }
            public RedBlackNode? left { get; set; }
        }

        private int _version = int.MinValue;
        
        private RedBlackNode root;
        private RedBlackNode TNULL = new RedBlackNode
        {
            color = 0
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{T}"/>.
        /// </summary>
        public RedBlackTree()
        {
            Comparer = Comparer<T>.Default;
            root = TNULL;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this red-black tree.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/></exception>
        public RedBlackTree(IComparer<T> comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            root = TNULL;
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
            
            root = TNULL;
            
            if (source is RedBlackTree<T> tree)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = tree.Count;
                Comparer = tree.Comparer;
                _version++;
                CloneRecursive(tree.root, root, tree._version, tree);
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
            
            root = TNULL;

            if (source is RedBlackTree<T> tree)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = tree.Count;
                _version++;
                CloneRecursive(tree.root, root, tree._version, tree);
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

            if (original.left != null)
            {
                @new.left = new RedBlackNode();
                CloneRecursive(original.left, @new.left, version, tree);
            }
                
            if (original.right != null)
            {
                @new.right = new RedBlackNode();
                CloneRecursive(original.right, @new.right, version, tree);
            }
        }

        /// <summary>
        /// The <see cref="IComparer{T}"/>
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <inheritdoc />
        public override bool TryAdd(T item)
        {
            Insert(item);
            Count++;
            _version++;
            return true;
        }
        
        /// <inheritdoc />
        public override bool Remove(T item)
        {
            DeleteNodeHelper(root, item);
            Count--;
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
            FreeRecursive(root);
            root = TNULL;
            Count = 0;
            _version = int.MinValue;
            
            static void FreeRecursive(RedBlackNode? node)
            {
                if (node == null)
                {
                    return;
                }
                
                FreeRecursive(node.left);
                FreeRecursive(node.right);

                node.left = null;
                node.right = null;
                node.parent = null;
                node.Value = default!;
            }
        }

        private RedBlackNode? Find(T item)
        {
            var current = root;
            while (current != null)
            {
                var result = Comparer.Compare(item, current.Value);
                if (result == 0)
                {
                    return current;
                }
                else if (result > 0)
                {
                    current = current.right;
                }
                else
                {
                    current = current.left;
                }
            }
            
            return null;
        }

       
        private void Insert(T item) 
        {
            var node = new RedBlackNode
            {
                parent = null,
                Value = item,
                left = TNULL,
                right = TNULL,
                color = 1
            };

            RedBlackNode? y = null;
            RedBlackNode x = root;

            while (x != TNULL) 
            {
                y = x;
                if (Comparer.Compare(node.Value, x.Value) < 0) 
                {
                    x = x.left!;
                } 
                else 
                {
                    x = x.right!;
                }
            }

            node.parent = y;
            if (y == null) 
            {
                root = node;
            } else if (Comparer.Compare(node.Value, y.Value) < 0) 
            {
                y.left = node;
            } else {
                y.right = node;
            }

            if (node.parent == null) 
            {
                node.color = 0;
                return;
            }

            if (node.parent.parent == null) 
            {
                return;
            }

            FixInsert(node);
        }
        
        private void FixInsert(RedBlackNode k) {
            RedBlackNode u;
            while (k.parent!.color == 1) {
                if (k.parent == k.parent.parent!.right) 
                {
                    u = k.parent.parent.left!;
                    if (u.color == 1) 
                    {
                        u.color = 0;
                        k.parent.color = 0;
                        k.parent.parent.color = 1;
                        k = k.parent.parent;
                    } 
                    else 
                    {
                        if (k == k.parent.left) 
                        {
                            k = k.parent;
                            RightRotate(k);
                        }
                        k.parent!.color = 0;
                        k.parent.parent!.color = 1;
                        LeftRotate(k.parent.parent);
                    }
                } 
                else 
                {
                    u = k.parent.parent.right!;

                    if (u.color == 1) 
                    {
                        u.color = 0;
                        k.parent.color = 0;
                        k.parent.parent.color = 1;
                        k = k.parent.parent;
                    } 
                    else 
                    {
                        if (k == k.parent.right) 
                        {
                            k = k.parent;
                            LeftRotate(k);
                        }
                        k.parent!.color = 0;
                        k.parent.parent!.color = 1;
                        RightRotate(k.parent.parent);
                    }
                }
                
                if (k == root) 
                {
                    break;
                }
            }
            root.color = 0;
        }

        private void LeftRotate(RedBlackNode x) 
        {
            var y = x.right!;
            x.right = y.left;
            if (y.left != TNULL) 
            {
                y.left.parent = x;
            }
            
            y.parent = x.parent;
            if (x.parent == null) 
            {
                root = y;
            } 
            else if (x == x.parent.left) 
            {
                x.parent.left = y;
            } 
            else {
                x.parent.right = y;
            }
            y.left = x;
            x.parent = y;
        }

        private void RightRotate(RedBlackNode x)
        {
            var y = x.left!;
            x.left = y.right;
            if (y.right != TNULL)
            {
                y.right.parent = x;
            }

            y.parent = x.parent;
            if (x.parent == null)
            {
                this.root = y;
            }
            else if (x == x.parent.right)
            {
                x.parent.right = y;
            }
            else
            {
                x.parent.left = y;
            }

            y.right = x;
            x.parent = y;
        }
        
        // Balance the tree after deletion of a node
        private void FixDelete(RedBlackNode x) 
        {
            RedBlackNode s;
            while (x != root && x.color == 0) {
                if (x == x.parent.left) {
                    s = x.parent.right;
                    if (s.color == 1) {
                        s.color = 0;
                        x.parent.color = 1;
                        LeftRotate(x.parent);
                        s = x.parent.right;
                    }

                    if (s.left.color == 0 && s.right.color == 0) {
                        s.color = 1;
                        x = x.parent;
                    } else {
                        if (s.right.color == 0) {
                            s.left.color = 0;
                            s.color = 1;
                            RightRotate(s);
                            s = x.parent.right;
                        }

                        s.color = x.parent.color;
                        x.parent.color = 0;
                        s.right.color = 0;
                        LeftRotate(x.parent);
                        x = root;
                    }
                } else {
                    s = x.parent.left;
                    if (s.color == 1) {
                        s.color = 0;
                        x.parent.color = 1;
                        RightRotate(x.parent);
                        s = x.parent.left;
                    }

                    if (s.right.color == 0 && s.right.color == 0) {
                        s.color = 1;
                        x = x.parent;
                    } else {
                        if (s.left.color == 0) {
                            s.right.color = 0;
                            s.color = 1;
                            LeftRotate(s);
                            s = x.parent.left;
                        }

                        s.color = x.parent.color;
                        x.parent.color = 0;
                        s.left.color = 0;
                        RightRotate(x.parent);
                        x = root;
                    }
                }
            }
            x.color = 0;
        }

        private void RbTransplant(RedBlackNode u, RedBlackNode v) 
        {
            if (u.parent == null) {
                root = v;
            } else if (u == u.parent.left) {
                u.parent.left = v;
            } else {
                u.parent.right = v;
            }
            v.parent = u.parent;
        }

        private void DeleteNodeHelper(RedBlackNode node, T value) 
        {
            var z = TNULL;
            RedBlackNode x, y;
            while (node != TNULL) {
                if (Comparer.Compare(node.Value, value) == 0) {
                    z = node;
                }

                if (Comparer.Compare(node.Value, value) <= 0) 
                {
                    node = node.right;
                } else {
                    node = node.left;
                }
            }

            if (z == TNULL) 
            {
                return;
            }

            y = z;
            int yOriginalColor = y.color;
            if (z.left == TNULL) {
                x = z.right;
                RbTransplant(z, z.right);
            } else if (z.right == TNULL) {
                x = z.left;
                RbTransplant(z, z.left);
            } else {
                y = Minimum(z.right);
                yOriginalColor = y.color;
                x = y.right;
                if (y.parent == z) {
                    x.parent = y;
                } else {
                    RbTransplant(y, y.right);
                    y.right = z.right;
                    y.right.parent = y;
                }

                RbTransplant(z, y);
                y.left = z.left;
                y.left.parent = y;
                y.color = z.color;
            }
            if (yOriginalColor == 0) {
                FixDelete(x);
            }
        }
        
        private RedBlackNode Minimum(RedBlackNode node) 
        {
            while (node.left != TNULL) {
                node = node.left;
            }
            return node;
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
                    _node = _tree.root;
                    _state = 0;
                }
                else if (_state == 0)
                {
                    _node = _node?.right;
                }
               
                if(_state != -2)
                {
                    while (!_stack.IsEmpty || _node != null || _node != _tree.TNULL)
                    {
                        if (_node != null)
                        {
                            _stack.Push(_node);
                            _node = _node.left;
                        }
                        else
                        {
                            _node = _stack.Pop();
                            break;
                        }
                    }
                }

                if (_state == -2 || _node == null || _node == _tree.TNULL)
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
                    _node = _tree.root;
                    _state = 0;
                }
                else if (_state == 0)
                {
                    if (_node!.left != null)
                    {
                        _stack.Push(_node);
                        _node = _node.left;
                    }
                    else if (_node.right != null)
                    {
                        _stack.Push(_node);
                        _node = _node.right;
                    }
                    else
                    {
                        var current = _stack.Pop();
                        while (current.right == null && !_stack.IsEmpty)
                        {
                            current = _stack.Pop();
                        }
                        
                        _node = current.right;
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
                    _node = _tree.root;
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
                            if (node.right == last)
                            {
                                _node = node;
                                break;
                            }

                            _stack.Push(node);
                            if (node.left == last)
                            {
                                last = node;
                                node = node.right;
                            }
                            else
                            {
                                last = node;
                                node = node.left;
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