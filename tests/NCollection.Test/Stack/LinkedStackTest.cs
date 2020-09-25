using System.Collections.Generic;

namespace NCollection.Test.Stack
{
    public class LinkedStackTestString : AbstractStackTest<string>
    {
        protected override bool ContainsInitialCapacity => false;
        protected override AbstractStack<string> CreateStack()
        {
            return new LinkedStack<string>();
        }

        protected override AbstractStack<string> CreateStack(IEnumerable<string> enumerable)
        {
            return new LinkedStack<string>(enumerable);
        }

        protected override AbstractCollection<string> CreateCollection(int size)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractCollection<string> CreateCollection(int size, IEnumerable<string> source)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class LinkedStackTestInt : AbstractStackTest<int>
    {
        protected override bool ContainsInitialCapacity => false;

        protected override AbstractStack<int> CreateStack()
        {
            return new LinkedStack<int>();
        }

        protected override AbstractStack<int> CreateStack(IEnumerable<int> enumerable)
        {
            return new LinkedStack<int>(enumerable); 
        }

        protected override AbstractCollection<int> CreateCollection(int size)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> source)
        {
            throw new System.NotImplementedException();
        }
    }
}