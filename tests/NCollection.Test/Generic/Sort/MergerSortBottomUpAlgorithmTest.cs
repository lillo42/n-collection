using NCollection.Generics;
using NCollection.Generics.Sorting.Merge;

namespace NCollection.Test.Generic.Sort
{
    public class MergerSortBottomUpAlgorithmTest : ISortAlgorithmTest
    {
        protected override ISortAlgorithm<T> Create<T>() => new MergerSortBottomUpAlgorithm<T>();
    }
}