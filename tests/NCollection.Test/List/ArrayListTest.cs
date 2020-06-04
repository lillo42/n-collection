using System.Collections;

namespace NCollection.Test.List
{
    public class ArrayListTest : IListTest
    {
        protected override IList Create() 
            => new ArrayList();

        protected override IList Create(IEnumerable values) 
            => new ArrayList(values);
    }
}