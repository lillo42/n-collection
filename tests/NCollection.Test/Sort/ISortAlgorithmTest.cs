using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Sort
{
    public abstract class ISortAlgorithmTest
    {
        protected abstract ISortAlgorithm Create();
        
        [Fact]
        public void ExecuteInt()
        {
            var a = new System.Collections.Generic.List<>();
            var numbers = new object[] {3, 7, 8, 5, 2, 1, 9, 5, 4};
            var sort = Create();
            sort.Sort(numbers, Comparer<int>.Default);

            numbers.Should().BeEquivalentTo(new[] {1, 2, 3, 4, 5, 5, 7, 8, 9});
        }
        
        [Fact]
        public void Sort_Should_Throw_When_ArrayIsNull()
        {
            var sort = Create();
            Assert.Throws<ArgumentNullException>(() => sort.Sort(null!, Comparer<int>.Default));
        }
        
        [Fact]
        public void Sort_Should_Throw_When_ComparerIsNull()
        {
            var numbers = new object[] {3, 7, 8, 5, 2, 1, 9, 5, 4};
            var sort = Create();
            Assert.Throws<ArgumentNullException>(() => sort.Sort(numbers, null!));
        }
    }
}