using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using NCollection.Generics;
using Xunit;

namespace NCollection.Test.Generic.Stack
{
    public class ArrayStackTest : IStackTest<string>
    {
        protected override IStack<string> Create() 
            => new ArrayStack<string>();

        protected override IStack<string> Create(IEnumerable<string> values) 
            => new ArrayStack<string>(values);

        [Fact]
        public void CopyArray_Should_Throw()
        {
            var stack = (ArrayStack<string>)Create();
            
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
        
        [Fact]
        public void CopyArray_Should_CopyTo_WhenArrayIsSameType()
        {
            var stack = (ArrayStack<string>)Create();
            
            var values = Fixture.Create<string[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }
            
            var newArray = new string[values.Length];
            stack.CopyTo((Array) newArray!, 0);
            
            var i = 0;
            while (stack.TryPop(out var value))
            {
                newArray[i++].Should().Be(value);
            }
        }
        
        [Fact]
        public void CopyArray_Should_CopyTo_WhenIsObjectArray()
        {
            var stack = (ArrayStack<string>)Create();
            
            var values = Fixture.Create<string[]>();

            foreach (var value in values)
            {
                stack.Push(value);
            }
            
            var newArray = new object[values.Length];
            stack.CopyTo((Array) newArray!, 0);
            
            var i = 0;
            while (stack.TryPop(out var value))
            {
                newArray[i++].Should().Be(value);
            }
        }
    }
}