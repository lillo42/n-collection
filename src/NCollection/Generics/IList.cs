using System;
using System.Collections.Generic;

namespace NCollection.Generics
{
    public interface IList<T> : System.Collections.Generic.IList<T>, ICollection<T>, ICloneable
    {
        /// <summary>
        /// Sort list based on algorithm.
        /// </summary>
        /// <param name="algorithm">The <see cref="ISortAlgorithm{T}"/>.</param>
        void Sort(ISortAlgorithm<T> algorithm)
            => Sort(algorithm, Comparer<T>.Default);

        /// <summary>
        /// Sort list based on algorithm.
        /// </summary>
        /// <param name="algorithm">The <see cref="ISortAlgorithm{T}"/>.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/>.</param>
        void Sort(ISortAlgorithm<T> algorithm, IComparer<T> comparer);
    }
}