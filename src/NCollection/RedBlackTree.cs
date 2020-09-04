using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedBlackTree<T> : AbstractTree<T>
    {
        private Node? _root;
        /// <summary>
        /// The <see cref="IComparer{T}"/>
        /// </summary>
        public IComparer<T> Comparer { get; }
        
        public override IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public override bool TryAdd(T item)
        {
            if (_root == null)
            {
                _root = new Node
                {
                    Value = item
                };
            }

            Count++;
            return true;
        }

        public override bool Remove(T item)
        {
            throw new System.NotImplementedException();
        }

        private static Node? GetParent(Node? node)
        {
            return node?.Parent;
        }
        
        private static Node? GetGrandParent(Node? node)
        {
            return GetParent(GetParent(node));
        }
        
        private static Node? GetSibling(Node? node)
        {
            var parent = GetParent(node);
            
            if (parent == null)
            {
                return null;
            }

            return node == parent.Left ? parent.Right : parent.Left;
        }
        
        private static Node? GetUncle(Node? node)
        {
            return GetSibling(GetParent(node));
        }

        private static void RotateLef([NotNull] Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var @new = node.Right ?? new Node();

            node.Right = @new.Left;
            @new.Left = node;

            if (node.Right != null)
            {
                node.Right.Parent = node;
            }
            
            var parent = GetParent(node);
            if (parent != null)
            {
                if (node == parent.Left)
                {
                    parent.Left = @new;
                }
                else if (node == parent.Right)
                {
                    parent.Right = @new;
                }
            }

            @new.Parent = parent;
        }
        
        private static void RotateRight([NotNull] Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var @new = node.Right ?? new Node();

            node.Left = @new.Right;
            @new.Right = node;

            if (node.Left != null)
            {
                node.Left.Parent = node;
            }
            
            var parent = GetParent(node);
            if (parent != null)
            {
                if (node == parent.Left)
                {
                    parent.Left = @new;
                }
                else if (node == parent.Right)
                {
                    parent.Right = @new;
                }
            }

            @new.Parent = parent;
        }
    
        private enum Color : byte
        {
            Black,
            Red
        }
        
        private class Node
        {
            
            public T Value { get; set; } = default!;
            
            public Color Color { get; set; } = Color.Black;
            public Node? Parent { get; set; }
            public Node? Right { get; set; }
            public Node? Left { get; set; }
        }
        
        
    }
}