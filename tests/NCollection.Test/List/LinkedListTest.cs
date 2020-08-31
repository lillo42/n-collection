using System.Collections.Generic;

namespace NCollection.Test.List
{
    public class LinkedListTest_String : AbstractListTest<string>
    {
        protected override bool ContainsInitialCapacity => false;
        
        protected override AbstractCollection<string> CreateCollection(int size)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractCollection<string> CreateCollection(int size, IEnumerable<string> enumerable)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractList<string> CreateList()
        {
            return new LinkedList<string>();
        }

        protected override AbstractList<string> CreateList(IEnumerable<string> enumerable)
        {
            return new LinkedList<string>(enumerable);
        }

    }
    
    public class LinkedListTest_Int : AbstractListTest<int>
    {
        protected override bool ContainsInitialCapacity => false;
        
        protected override AbstractCollection<int> CreateCollection(int size)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> enumerable)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractList<int> CreateList()
        {
            return new LinkedList<int>();
        }

        protected override AbstractList<int> CreateList(IEnumerable<int> enumerable)
        {
            return new LinkedList<int>(enumerable);
        }

    }
}