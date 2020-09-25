using System.Collections.Generic;

namespace NCollection.Test.List
{
    public class ArrayListTestString : AbstractListTest<string>
    {
        protected override AbstractCollection<string> CreateCollection(int size)
        {
            return new ArrayList<string>(size);
        }

        protected override AbstractCollection<string> CreateCollection(int size, IEnumerable<string> source)
        {
            return new ArrayList<string>(size, source);
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
    
    public class ArrayListTestInt : AbstractListTest<int>
    {
        protected override AbstractCollection<int> CreateCollection(int size)
        {
            return new ArrayList<int>(size);
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> source)
        {
            return new ArrayList<int>(size, source);
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