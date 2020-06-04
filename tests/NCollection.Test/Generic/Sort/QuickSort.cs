using System.Collections.Generic;
using FluentAssertions;
using NCollection.Generics.Sorting;
using Xunit;

namespace NCollection.Test.Generic.Sort
{
    public class QuickSort
    {
        
        [Fact]
        public void SortInt()
        {
            var numbers = new [] {3, 7, 8, 5, 2, 1, 9, 5, 4};
            var sort = new QuickSortAlgorithm<int>();
            sort.Execute(numbers, Comparer<int>.Default);

            numbers.Should().BeEquivalentTo(new[] {1, 2, 3, 4, 5, 5, 7, 8, 9});
        }
    }
}