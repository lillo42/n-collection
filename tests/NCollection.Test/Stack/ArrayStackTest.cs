namespace NCollection.Test.Stack
{
    public class ArrayStackTest_String : AbstractStackTest<string>
    {
        protected override AbstractStack<string> CreateStack()
        {
            return new ArrayStack<string>();
        }
    }
    
    public class ArrayStackTest_Int : AbstractStackTest<int>
    {
        protected override AbstractStack<int> CreateStack()
        {
            return new ArrayStack<int>();
        }
    }
}