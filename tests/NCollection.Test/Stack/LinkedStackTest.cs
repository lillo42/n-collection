namespace NCollection.Test.Stack
{
    public class LinkedStackTest_String : AbstractStackTest<string>
    {
        protected override AbstractStack<string> CreateStack()
        {
            return new LinkedStack<string>();
        }
    }
    
    public class LinkedStackTest_Int : AbstractStackTest<int>
    {
        protected override AbstractStack<int> CreateStack()
        {
            return new LinkedStack<int>();
        }
    }
}