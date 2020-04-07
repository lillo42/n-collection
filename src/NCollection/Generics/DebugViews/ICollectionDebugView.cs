using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCollection.Generics.DebugViews
{
    internal sealed class ICollectionDebugView<T>
    {
        private readonly ICollection<T> _collection;

        public ICollectionDebugView(ICollection<T> stack)
        {
            _collection = stack ?? throw new ArgumentNullException(nameof(stack));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var items = new T[_collection.Count];
                _collection.CopyTo(items, 0);
                return items;
            }
        }
    }
}