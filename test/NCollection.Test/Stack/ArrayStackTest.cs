namespace NCollection.Test.Stack
{
    public class ArrayStackTest : IStackTest
    {
        protected override IStack Create() 
            => new ArrayStack();
    }
}