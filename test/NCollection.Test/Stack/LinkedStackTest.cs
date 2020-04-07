using System.Collections;

namespace NCollection.Test.Stack
{
    public class LinkedStackTest : IStackTest
    {
        protected override IStack Create() 
            => new LinkedStack();

        protected override IStack Create(IEnumerable values) 
            => new LinkedStack(values);
    }
}