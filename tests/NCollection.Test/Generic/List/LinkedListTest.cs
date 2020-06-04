using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NCollection.Test.Generic.List
{
    public class LinkedListTest : IListTest<string>
    {
        protected override NCollection.Generics.IList<string> Create() 
            => new Generics.LinkedList<string>();

        protected override NCollection.Generics.IList<string> Create(IEnumerable<string> values) 
            => new Generics.LinkedList<string>(values);

        [Fact]
        public void IsReadOnly_Should_BeFalse()
        {
            var list = Create();
            list.IsReadOnly.Should().BeFalse();
        }
    }
}