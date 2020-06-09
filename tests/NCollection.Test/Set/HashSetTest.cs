using System.Collections;

namespace NCollection.Test.Set
{
    public class HashSetTest : ISetTest
    {
        protected override ISet Create() => new HashSet();

        protected override ISet Create(IEnumerable values) => new HashSet(values);
    }
}