using System.Collections.Generic;
 
namespace NCollection.Test.Set
{
    public abstract class HashSetTest<T> : AbstractSetTest<T>
    {
        
    }

    public class HashSetInt : HashSetTest<int>
    {
        protected override AbstractCollection<int> CreateCollection(int size)
        {
            return new HashSet<int>(size);
        }

        protected override AbstractCollection<int> CreateCollection(int size, IEnumerable<int> source)
        {
            return new HashSet<int>(source);
        }

        protected override AbstractSet<int> CreateSet()
        {
            return new HashSet<int>();
        }

        protected override AbstractSet<int> CreateSet(IEnumerable<int> source)
        {
            return new HashSet<int>(source);
        }
    }

    public class HashSetString : HashSetTest<string>
    {
        protected override AbstractCollection<string> CreateCollection(int size)
        {
            return new HashSet<string>(size);
        }

        protected override AbstractCollection<string> CreateCollection(int size, IEnumerable<string> source)
        {
            return new HashSet<string>(source);
        }

        protected override AbstractSet<string> CreateSet()
        {
            return new HashSet<string>();
        }

        protected override AbstractSet<string> CreateSet(IEnumerable<string> source)
        {
            return new HashSet<string>(source);
        }
    }
}
