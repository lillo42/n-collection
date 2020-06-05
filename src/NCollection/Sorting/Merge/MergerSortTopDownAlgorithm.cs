using System;
using System.Collections;

namespace NCollection.Sorting.Merge
{
    /// <summary>
    /// Merge sort: https://en.wikipedia.org/wiki/Merge_sort
    /// </summary>
    public class MergerSortTopDownAlgorithm : ISortAlgorithm
    {
        /// <inheritdoc />
        public void Sort(object[] array, IComparer comparer)
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


        private static void MergeSort(Span<object> source, IComparer comparer)
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


        private static void Merge(Span<object> source, Span<object> array1, Span<object> array2, IComparer comparer)
        {
            var left = 0;
            var right = 0;
            var index = 0;
            var store = new object[source.Length];
            
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