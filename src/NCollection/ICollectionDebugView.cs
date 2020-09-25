using System;
using System.Diagnostics;

namespace NCollection
{
    /// <summary>
    /// The internal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // ReSharper disable once InconsistentNaming
    internal sealed class ICollectionDebugView<T>
    {
        private readonly ICollection<T> _collection;

        public ICollectionDebugView(ICollection<T> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _collection.ToArray();
    }
}