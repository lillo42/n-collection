using System.Collections;

namespace NCollection
{
    /// <summary>
    /// Represent sort algorithm
    /// </summary>
    public interface ISortAlgorithm
    {
        /// <summary>
        /// Execute sorting
        /// </summary>
        /// <param name="array">The array to be sort</param>
        /// <param name="comparer">The <see cref="IComparer"/>.</param>
        void Sort(object[] array, IComparer comparer);
    }
}