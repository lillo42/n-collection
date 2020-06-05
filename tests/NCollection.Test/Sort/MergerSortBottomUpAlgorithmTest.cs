using NCollection.Sorting.Merge;

namespace NCollection.Test.Sort
{
    public class MergerSortBottomUpAlgorithmTest : ISortAlgorithmTest
    {
        protected override ISortAlgorithm Create() => new MergerSortBottomUpAlgorithm();
    }
}