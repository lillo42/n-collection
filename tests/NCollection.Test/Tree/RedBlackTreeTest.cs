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
            throw new InvalidOperationException();
        }

        protected override AbstractCollection<T> CreateCollection(int size, IEnumerable<T> source)
        {
            throw new InvalidOperationException();
        }

        protected override AbstractTree<T> CreateTree()
        {
            return new RedBlackTree<T>();
        }

        protected override AbstractTree<T> CreateTree(IEnumerable<T> enumerable)
        {
            return new RedBlackTree<T>(enumerable);
        }
        
        [Fact]
        public void Constructor_Throw_When_Comparer_IsNull()
        {
            IComparer<T> cmp = null;
            Assert.Throws<ArgumentNullException>(() => new RedBlackTree<T>(cmp));
            Assert.Throws<ArgumentNullException>(() => new RedBlackTree<T>(cmp, new T[0]));
        }

        [Fact]
        public abstract void Inorder();
        
        [Fact]
        public abstract void Postorder();
        
        [Fact]
        public abstract void Preorder();
    }
    
    public class RedBlackTreeTestInt : RedBlackTreeTest<int>
    {
        public override void Inorder()
        {
            var array = new[] {54, 74, 172, 109, 154, 52, 127, 123, 171, 8};
            var tree = CreateTree(array);
            var expected = new[] {8, 52, 54, 74, 109, 123, 127, 154, 171, 172};
            var enumerable = tree.InorderTraversal();

            var index = 0;
            while (enumerable.MoveNext())
            {
                enumerable.Current.Should().Be(expected[index]);
                index++;
            }

            index.Should().Be(expected.Length);
        }

        public override void Postorder()
        {
            var array = new[] {54, 74, 172, 109, 154, 52, 127, 123, 171, 8};
            var tree = CreateTree(array);
            var expected = new[] {8, 54, 52, 109, 127, 123, 171, 172, 154, 74};
            var enumerable = tree.PostorderTraversal();

            var index = 0;
            while (enumerable.MoveNext())
            {
                enumerable.Current.Should().Be(expected[index]);
                index++;
            }

            index.Should().Be(expected.Length);
        }

        public override void Preorder()
        {
            var array = new[] {54, 74, 172, 109, 154, 52, 127, 123, 171, 8};
            var tree = CreateTree(array);
            var expected = new[] {74, 52, 8, 54, 154, 123, 109, 127, 172, 171};
            var enumerable = tree.PreorderTraversal();

            var index = 0;
            while (enumerable.MoveNext())
            {
                enumerable.Current.Should().Be(expected[index]);
                index++;
            }

            index.Should().Be(expected.Length);
        }
    }

    public class RedBlackTreeTestString : RedBlackTreeTest<string>
    {
        public override void Inorder()
        {
            var array = new[]
            {
                "muscle", "unit", "novel", "runner", "electronics", "gold", 
                "recovery", "cable", "photograph", "conglomerate"
            };
            var tree = CreateTree(array);
            var expected = new[]
            {
                "cable", "conglomerate", "electronics", "gold", "muscle", 
                "novel", "photograph", "recovery", "runner", "unit"
            };
            var enumerable = tree.InorderTraversal();

            var index = 0;
            while (enumerable.MoveNext())
            {
                enumerable.Current.Should().Be(expected[index]);
                index++;
            }

            index.Should().Be(expected.Length);
        }

        public override void Postorder()
        {
            var array = new[]
            {
                "muscle", "unit", "novel", "runner", "electronics", "gold", 
                "recovery", "cable", "photograph", "conglomerate"
            };
            
            var tree = CreateTree(array);
            var expected = new[]
            {
                "cable", "electronics", "conglomerate", "muscle", "gold", 
                "photograph", "recovery", "unit", "runner", "novel"
            };
            
            
            var enumerable = tree.PostorderTraversal();
            
            var index = 0;
            while (enumerable.MoveNext())
            {
                enumerable.Current.Should().Be(expected[index]);
                
                index++;
            }
            
            index.Should().Be(expected.Length);
        }

        public override void Preorder()
        {
            var array = new[]
            {
                "muscle", "unit", "novel", "runner", "electronics", "gold", 
                "recovery", "cable", "photograph", "conglomerate"
            };
            
            var tree = CreateTree(array);
            
            var expected = new[]
            {
                "novel", "gold", "conglomerate", "cable", "electronics",
                "muscle", "runner", "recovery", "photograph", "unit"
            };
            
            var enumerable = tree.PreorderTraversal();
            
            var index = 0;
            while (enumerable.MoveNext())
            {
                enumerable.Current.Should().Be(expected[index]);
                index++;
            }
            
            index.Should().Be(expected.Length);
        }
    }
}