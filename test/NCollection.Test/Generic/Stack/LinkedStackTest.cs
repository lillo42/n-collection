using NCollection.Generics;

namespace NCollection.Test.Generic.Stack
{
    public class LinkedStackTest : IStackTest<string>
    {
        protected override IStack<string> Create() 
            => new LinkedStack<string>();
    }
}