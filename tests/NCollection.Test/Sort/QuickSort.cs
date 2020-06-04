using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using NCollection.Sorting;
using Xunit;

namespace NCollection.Test.Sort
{
    public class QuickSort
    {
        
        [Fact]
        public void SortInt()
        {
            var numbers = new object[] {3, 7, 8, 5, 2, 1, 9, 5, 4};
            var sort = new QuickSortAlgorithm();
            sort.Execute(numbers, Comparer<int>.Default);

            numbers.Should().BeEquivalentTo(new[] {1, 2, 3, 4, 5, 5, 7, 8, 9});
        }
    }
}