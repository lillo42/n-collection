using System.Collections.Generic;

namespace NCollection.Test.Stack
{
    public class ArrayStackTest_String : AbstractStackTest<string>
    {
        protected override AbstractStack<string> CreateStack(IEnumerable<string> enumerable)
        {
            return new ArrayStack<string>(enumerable);
        }

        protected override AbstractStack<string> CreateStack()
        {
            return new ArrayStack<string>();
        }

        protected override AbstractCollection<string> CreateCollection(int size)
        {
            return new ArrayStack<string>(size);
        }

        protected override AbstractCollection<string> CreateCollection(int size, IEnumerable<string> enumerable)
        {
            return new ArrayStack<string>(size, enumerable);
        }
    }
    
    public class ArrayStackTest_Int : AbstractStackTest<int>
    {
        protected override AbstractStack<int> CreateStack(IEnumerable<int> enumerable)
        {
            return new ArrayStack<int>(enumerable);
        }
        
        protected override AbstractStack<int> CreateStack()
        {
            return new ArrayStack<int>();
        }
        
        protected override AbstractCollection<int> CreateCollection(int size)
        {
            return new ArrayStack<int>(size);
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> enumerable)
        {
            return new ArrayStack<int>(size, enumerable);
        }
    }
}