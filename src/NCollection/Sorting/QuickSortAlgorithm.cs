using System;
using System.Collections;

namespace NCollection.Sorting
{
    /// <summary>
    /// Quick sort
    /// </summary>
    public class QuickSortAlgorithm : ISortAlgorithm
    {
        /// <inheritdoc />
        public void Execute(object[] array, IComparer comparer)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            
            QuickSort(array, comparer, 0, array.Length - 1);
        }

        private static void QuickSort(object[] array, IComparer comparer, int begin, int end)
        {
            if (begin < end)
            {
                var partition = Partition(array, comparer, begin, end);
                QuickSort(array, comparer, begin, partition - 1);
                QuickSort(array, comparer, partition + 1, end);
            }
        }

        private static int Partition(object[] array, IComparer comparer, int begin, int end)
        {
            var pivot = array[end];
            var i = (begin - 1);
            for (var j = begin; j < end; j++)
            {
                if (comparer.Compare(array[j], pivot) <= 0)
                {
                    i++;
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }

            i++;
            (array[i], array[end]) = (array[end], array[i]);
            return i;
        }
    }
}