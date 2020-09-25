namespace NCollection
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractSet<T> : AbstractCollection<T>, ISet<T>
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractSet{T}"/>.
        /// </summary>
        protected AbstractSet()
            : this(Comparer<T>.Default)
        {
            
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractSet{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> that will be used to order this set.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="comparer"/> is <see langword="null"/>.</exception>
        protected AbstractSet([NotNull] IComparer<T> comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        /// <summary>
        /// The <see cref="IComparer{T}"/>
        /// </summary>
        public virtual IComparer<T> Comparer { get; }

        public abstract bool Add(T item);
        public abstract void ExceptWith(IEnumerable<T> other);
        public abstract void IntersectWith(IEnumerable<T> other);
        public abstract bool IsProperSubsetOf(IEnumerable<T> other);
        public abstract bool IsProperSupersetOf(IEnumerable<T> other);
        public abstract bool IsSubsetOf(IEnumerable<T> other);
        public abstract bool IsSupersetOf(IEnumerable<T> other);
        public abstract bool Overlaps(IEnumerable<T> other);
        public abstract bool SetEquals(IEnumerable<T> other);
        public abstract void SymmetricExceptWith(IEnumerable<T> other);
        public abstract void UnionWith(IEnumerable<T> other);
    }
}
