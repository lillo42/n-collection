using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using NCollection.Generics;
using Xunit;

namespace NCollection.Test.Generic.List
{
    public class ArrayListTest : IListTest<string>
    {
        protected override NCollection.Generics.IList<string> Create() 
            => new ArrayList<string>();

        protected override NCollection.Generics.IList<string> Create(IEnumerable<string> values) 
            => new ArrayList<string>(values);
        
        [Fact]
        public void ConstructorWithInitialCapacity()
        {
            var list = new ArrayList(16);
            list.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void ConstructorWithInitialCapacity_Should_Throw_When_IsInitialCapacityLessThan0() 
            => Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayList(-1));
        
        [Fact]
        public void IsReadOnly_Should_BeFalse()
        {
            var list = Create();
            list.IsReadOnly.Should().BeFalse();
        }
    }
}