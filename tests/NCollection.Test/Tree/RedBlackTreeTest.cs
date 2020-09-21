using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Tree
{
    public abstract class RedBlackTreeTest<T> : AbstractTreeTest<T>
    {
        protected override bool ContainsInitialCapacity => false;

        protected override AbstractCollection<T> CreateCollection(int size)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractCollection<T> CreateCollection(int size, IEnumerable<T> source)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractTree<T> CreateTree()
        {
            return new RedBlackTree<T>();
        }

        protected override AbstractTree<T> CreateTree(IEnumerable<T> enumerable)
        {
            return new RedBlackTree<T>(enumerable);
        }

        [Theory]
        [InlineData(3)]
        public void IsBalance(int length)
        {
            var array = CreateAValidArray(length);
            var tree = new RedBlackTree<T>(array);

            var left = Height(tree._root!.Left);
            var right = Height(tree._root.Right);

            left.Should().Be(right);

            static int Height(RedBlackTree<T>.RedBlackNode node)
            {
                if (node == null)
                {
                    return 0;
                }

                var left = Height(node.Left);
                var right = Height(node.Right);
                return Math.Max(left, right) + 1;
            }
        }
        
        [Fact]
        public void Constructor_Throw_When_Comparer_IsNull()
        {
            IComparer<T> cmp = null;
            Assert.Throws<ArgumentNullException>(() => new RedBlackTree<T>(cmp));
            Assert.Throws<ArgumentNullException>(() => new RedBlackTree<T>(cmp, new T[0]));
        }

        //[Fact]
        public abstract void Inorder();
        
        //[Fact]
        public abstract void Postorder();
        
        //[Fact]
        public abstract void Preorder();
    }
    
    public class RedBlackTreeTest_Int : RedBlackTreeTest<int>
    {
        public override void Inorder()
        {
            throw new NotImplementedException();
        }

        public override void Postorder()
        {
            throw new NotImplementedException();
        }

        public override void Preorder()
        {
            throw new NotImplementedException();
        }
    }

    public class RedBlackTreeTest_String : RedBlackTreeTest<string>
    {
        public override void Inorder()
        {
            throw new NotImplementedException();
        }

        public override void Postorder()
        {
            throw new NotImplementedException();
        }

        public override void Preorder()
        {
            throw new NotImplementedException();
        }
    }
}