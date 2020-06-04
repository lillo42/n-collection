using System;
using System.Collections.Generic;
using FluentAssertions;
using NCollection.Generics;
using Xunit;

namespace NCollection.Test.Generic.Sort
{
    public abstract class ISortAlgorithmTest
    {
        protected abstract ISortAlgorithm<T> Create<T>();
        
        [Fact]
        public void ExecuteInt()
        {
            var numbers = new[] {3, 7, 8, 5, 2, 1, 9, 5, 4};
            var sort = Create<int>();
            sort.Execute(numbers, Comparer<int>.Default);

            numbers.Should().BeEquivalentTo(new[] {1, 2, 3, 4, 5, 5, 7, 8, 9});
        }
        
        [Fact]
        public void ISortAlgorithm_ExecuteInt()
        {
            var numbers = new object[] {3, 7, 8, 5, 2, 1, 9, 5, 4};
            var sort = (ISortAlgorithm)Create<int>();
            sort.Execute(numbers, Comparer<int>.Default);

            numbers.Should().BeEquivalentTo(new[] {1, 2, 3, 4, 5, 5, 7, 8, 9});
        }
        
        [Fact]
        public void Execute_Should_Throw_When_ArrayIsNull()
        {
            var sort = Create<int>();
            Assert.Throws<ArgumentNullException>(() => sort.Execute(null!, Comparer<int>.Default));
        }
        
        [Fact]
        public void Execute_Should_Throw_When_ComparerIsNull()
        {
            var numbers = new object[] {3, 7, 8, 5, 2, 1, 9, 5, 4};
            var sort = Create<int>();
            Assert.Throws<ArgumentNullException>(() => sort.Execute(numbers, null!));
        }
    }
}