using NCollection.Sorting;

namespace NCollection.Test.Sort
{
    public class QuickSortAlgorithmTest : ISortAlgorithmTest
    {
        protected override ISortAlgorithm Create()
        {
            return new QuickSortAlgorithm();
        }
    }
}