using System;
using System.Diagnostics;

namespace NCollection.DebugViews
{
    internal sealed class ICollectionDebugView
    {
        private readonly ICollection _collection;

        public ICollectionDebugView(ICollection stack)
        {
            _collection = stack ?? throw new ArgumentNullException(nameof(stack));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object[] Items
        {
            get
            {
                var items = new object[_collection.Count];
                _collection.CopyTo(items, 0);
                return items;
            }
        }
    }
}