using System.Collections;

namespace NCollection
{
    public interface ISortAlgorithm
    {
        void Execute(object[] array, IComparer comparer);

    }
}