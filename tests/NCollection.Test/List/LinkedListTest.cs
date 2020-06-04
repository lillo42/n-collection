using System.Collections;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.List
{
    public class LinkedListTest : IListTest
    {
        protected override IList Create() 
            => new LinkedList();

        protected override IList Create(IEnumerable values) 
            => new LinkedList(values);
        
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