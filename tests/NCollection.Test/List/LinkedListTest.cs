using System.Collections;

namespace NCollection.Test.List
{
    public class LinkedListTest : IListTest
    {
        protected override IList Create() 
            => new LinkedList();

        protected override IList Create(IEnumerable values) 
            => new LinkedList(values);
    }
}