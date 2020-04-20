using System.Collections;

namespace NCollection.Test.Stack
{
    public class ArrayStackTest : IStackTest
    {
        protected override IStack Create() 
            => new ArrayStack();

        protected override IStack Create(IEnumerable values) 
            => new ArrayStack(values);
    }
}