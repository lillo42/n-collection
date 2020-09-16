using System.Collections.Generic;

namespace NCollection.Test.Tree
{
    public abstract class AbstractTreeTest<T> : AbstractionCollectionTest<T>
    {
        protected abstract AbstractTree<T> CreateTree();
        protected abstract AbstractTree<T> CreateTree(IEnumerable<T> enumerable);

        protected override AbstractCollection<T> CreateCollection()
        {
            return CreateTree();
        }

        protected override AbstractCollection<T> CreateCollection(IEnumerable<T> array)
        {
            return CreateTree(array);
        }
    }
}