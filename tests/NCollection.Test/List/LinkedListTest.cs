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

        protected override AbstractCollection<string> CreateCollection(int size, IEnumerable<string> source)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractList<string> CreateList()
        {
            return new LinkedList<string>();
        }

        protected override AbstractList<string> CreateList(IEnumerable<string> source)
        {
            return new LinkedList<string>(source);
        }

    }
    
    public class LinkedListTest_Int : AbstractListTest<int>
    {
        protected override bool ContainsInitialCapacity => false;
        
        protected override AbstractCollection<int> CreateCollection(int size)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> source)
        {
            throw new System.NotImplementedException();
        }

        protected override AbstractList<int> CreateList()
        {
            return new LinkedList<int>();
        }

        protected override AbstractList<int> CreateList(IEnumerable<int> source)
        {
            return new LinkedList<int>(source);
        }

    }
}