using NCollection.Generics;
using NCollection.Generics.Sorting.Merge;

namespace NCollection.Test.Generic.Sort
{
    public class MergerSortTopDownAlgorithmTest : ISortAlgorithmTest
    {
        protected override ISortAlgorithm<T> Create<T>() => new MergerSortTopDownAlgorithm<T>();
    }
}