using System.Collections.Generic;

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
    }
    
    public class RedBlackTreeTest_Int : RedBlackTreeTest<int> {}
}