using NCollection.Generics;
using NCollection.Generics.Sorting;

namespace NCollection.Test.Generic.Sort
{
    public class QuickSortAlgorithmTest : ISortAlgorithmTest
    {
        protected override ISortAlgorithm<T> Create<T>()
        {
            return new QuickSortAlgorithm<T>();
        }
    }
}