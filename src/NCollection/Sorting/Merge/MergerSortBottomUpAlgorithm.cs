using System;
using System.Collections;

namespace NCollection.Sorting.Merge
{
    /// <summary>
    /// Merge sort: https://en.wikipedia.org/wiki/Merge_sort
    /// </summary>
    public class MergerSortBottomUpAlgorithm : ISortAlgorithm
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
            var width = 1;
            var length = source.Length;

            while (width < length)
            {
                var i = 0;

                while (i < length)
                {
                    var upper = Math.Min(i + 2 * width, length);
                    var mid = Math.Min(i + width, length);
                    
                    Merge(source[i..upper], source[i..mid], source[mid..upper], comparer);

                    i += 2 * width;
                }
                
                width *= 2;
            }
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