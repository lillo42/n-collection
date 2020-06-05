using NCollection.Sorting.Merge;

namespace NCollection.Test.Sort
{
    public class MergerSortTopDownAlgorithmTest : ISortAlgorithmTest
    {
        protected override ISortAlgorithm Create() => new MergerSortTopDownAlgorithm();
    }
}