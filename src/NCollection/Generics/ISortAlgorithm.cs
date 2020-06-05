using System;
using System.Collections;
using System.Collections.Generic;

namespace NCollection.Generics
{
    /// <summary>
    /// Represent sort algorithm
    /// </summary>
    public interface ISortAlgorithm<T> : ISortAlgorithm
    {
        /// <summary>
        /// Execute sorting
        /// </summary>
        /// <param name="array">The array to be sort</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/>.</param>
        void Sort(T[] array, IComparer<T> comparer);

        void ISortAlgorithm.Sort(object[] array, IComparer comparer)
        {
            var newArray = new T[array.Length];
            Sort(newArray, (IComparer<T>)comparer);
            Array.Copy(array, newArray, array.Length);
        }
    }
}