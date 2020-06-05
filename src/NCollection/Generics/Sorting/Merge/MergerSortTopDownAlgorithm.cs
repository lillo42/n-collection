using System;
using System.Collections;
using System.Collections.Generic;

namespace NCollection.Generics.Sorting.Merge
{
    /// <summary>
    /// Merge sort: https://en.wikipedia.org/wiki/Merge_sort
    /// </summary>
    public class MergerSortTopDownAlgorithm<T> : ISortAlgorithm<T>
    {
        /// <inheritdoc />
        public void Sort(T[] array, IComparer<T> comparer)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            MergeSort(array, comparer);
        }


        private static void MergeSort(Span<T> source, IComparer<T> comparer)
        {
            var mid = source.Length / 2;
            if (mid == 0)
            {
                return;
            }

            MergeSort(source[mid..], comparer);
            MergeSort(source[..mid], comparer);
            
            Merge(source, source[mid..], source[..mid], comparer);
        }


        private static void Merge(Span<T> source, Span<T> array1, Span<T> array2, IComparer<T> comparer)
        {
            var left = 0;
            var right = 0;
            var index = 0;
            var store = new T[source.Length];
            
            while(left < array1.Length && right < array2.Length)
            {
                if (comparer.Compare(array1[left], array2[right]) <= 0)
                {
                    store[index] = array1[left];
                    left++;
                }
                else
                {
                    store[index] = array2[right];
                    right++;
                }

                index++;
            }

            while (left < array1.Length)
            {
                store[index++] = array1[left++];
            }

            while (right < array2.Length)
            {
                store[index++] = array2[right++];
            }
            
            store.CopyTo(source);
        }
    }     
}