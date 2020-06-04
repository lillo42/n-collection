using System;
using System.Collections;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.List
{
    public class ArrayListTest : IListTest
    {
        protected override IList Create() 
            => new ArrayList();

        protected override IList Create(IEnumerable values) 
            => new ArrayList(values);
        
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
        public void IsSynchronized_Should_BeFalse()
        {
            var list = Create();
            list.IsSynchronized.Should().BeFalse();
        }
        
        [Fact]
        public void SyncRoot_Should_BeFalse()
        {
            var list = Create();
            list.SyncRoot.Should().Be(list);
        }
        
        [Fact]
        public void IsFixedSize_Should_BeFalse()
        {
            var list = Create();
            list.IsFixedSize.Should().BeFalse();
        }
        
        [Fact]
        public void IsReadOnly_Should_BeFalse()
        {
            var list = Create();
            list.IsReadOnly.Should().BeFalse();
        }
    }
}