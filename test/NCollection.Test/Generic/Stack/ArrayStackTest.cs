using NCollection.Generics;

namespace NCollection.Test.Generic.Stack
{
    public class ArrayStackTest : IStackTest<string>
    {
        protected override IStack<string> Create() 
            => new ArrayStack<string>();
    }
}