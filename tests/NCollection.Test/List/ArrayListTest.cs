using System.Collections.Generic;

namespace NCollection.Test.List
{
    public class ArrayListTest_String : AbstractListTest<string>
    {
        protected override AbstractCollection<string> CreateCollection(int size)
        {
            return new ArrayList<string>(size);
        }

        protected override AbstractCollection<string> CreateCollection(int size, IEnumerable<string> enumerable)
        {
            return new ArrayList<string>(size, enumerable);
        }

        protected override AbstractList<string> CreateList()
        {
            return new ArrayList<string>();
        }

        protected override AbstractCollection<string> CreateCollection(IEnumerable<string> array)
        {
            return new ArrayList<string>(array);
        }
    }
    
    public class ArrayListTest_Int : AbstractListTest<int>
    {
        protected override AbstractCollection<int> CreateCollection(int size)
        {
            return new ArrayList<int>(size);
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> enumerable)
        {
            return new ArrayList<int>(size, enumerable);
        }

        protected override AbstractList<int> CreateList()
        {
            return new ArrayList<int>();
        }

        protected override AbstractCollection<int> CreateCollection(IEnumerable<int> array)
        {
            return new ArrayList<int>(array);
        }
    }
}