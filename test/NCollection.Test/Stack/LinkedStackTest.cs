namespace NCollection.Test.Stack
{
    public class LinkedStackTest : IStackTest
    {
        protected override IStack Create() 
            => new LinkedStack();
    }
}