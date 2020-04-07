using System;
using System.Collections.Generic;
using AutoFixture;
using NCollection.Generics;
using Xunit;

namespace NCollection.Test.Generic.Stack
{
    public class LinkedStackTest : IStackTest<string>
    {
        protected override IStack<string> Create() 
            => new LinkedStack<string>();

        protected override IStack<string> Create(IEnumerable<string> values) 
            => new LinkedStack<string>(values);
        
        [Fact]
        public void CopyArray_Should_Throw()
        {
            var stack = (LinkedStack<string>)Create();
            
            var values = Fixture.Create<string[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }
            
            Assert.Throws<ArgumentNullException>(() => stack.CopyTo((Array)null!, values.Length));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.CopyTo((Array)new string[10], -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.CopyTo((Array)new string[10], values.Length + 1));
            Assert.Throws<ArgumentException>(() => stack.CopyTo((Array)new string[1], values.Length));
        }
    }
}