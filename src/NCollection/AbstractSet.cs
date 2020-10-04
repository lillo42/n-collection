using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NCollection
{
    /// <summary>
    /// This class provides a skeletal implementation of the <see cref="ISet{T}"/>
    /// interface, to minimize the effort required to implement this interface. 
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public abstract class AbstractSet<T> : AbstractCollection<T>, ISet<T>
    {
        // store lower 31 bits of hash code
        private const int Lower31BitMask = 0x7FFFFFFF;
        
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
            // ReSharper disable once VirtualMemberCallInConstructor
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }
        
        /// <summary>
        /// The <see cref="IComparer{T}"/>
        /// </summary>
        public virtual IComparer<T> Comparer { get; protected set; }

        /// <summary>
        /// Generate hash code of <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item to get hash code</param>
        /// <returns>The hash code of <paramref name="item"/>.</returns>
        protected virtual int Hash(T item)
        {
            if (item == null)
            {
                return 0;
            }
            
            return item.GetHashCode() & Lower31BitMask;
        }
        
        /// <inheritdoc  cref="System.Collections.Generic.ISet{T}.Add" />
        public virtual new bool Add(T item) => TryAdd(item);
        
        /// <inheritdoc />
        public virtual void ExceptWith(IEnumerable<T> other) 
            => RetainAll(other);
        
        /// <inheritdoc />
        public virtual bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            if (Equals(other, this))
            {
                return false;
            }

            if (other is System.Collections.Generic.ICollection<T> collection)
            {
                if (collection.Count == 0)
                {
                    return false;
                }

                if (Count == 0)
                {
                    return collection.Count > 0;
                }

                if (other is AbstractSet<T> set && AreComparersEqual(this, set))
                {
                    if (Count >= set.Count)
                    {
                        return false;
                    }
                    
                    return IsSubsetOfHashSetWithSameEc(set);
                }
            }
            
            var (uniqueCount, unfoundCount) = CheckUniqueAndUnfoundElements(other, false);
            return uniqueCount == Count && unfoundCount > 0;
        }

        /// <inheritdoc />
        public virtual void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Count == 0 || Equals(other, this))
            {
                return;
            }

            if (other is System.Collections.Generic.ICollection<T> collection)
            {
                if (collection.Count == 0)
                {
                    Clear();
                    return;
                }

                if (other is AbstractSet<T> set && AreComparersEqual(this, set))
                {
                    IntersectWithHashSetWithSameEC(set);
                    return;
                }
            }

            IntersectWithEnumerable(other);
        }
        
        /// <summary>
        /// If other is a hashset that uses same equality comparer, intersect is much faster 
        /// because we can use other's Contains
        /// </summary>
        /// <param name="other"></param>
        protected virtual void IntersectWithHashSetWithSameEC(AbstractSet<T> other)
        {
            foreach (var item in this)
            {
                if (!other.Contains(item))
                {
                    Remove(item);
                }
            }
        }
        
        /// <summary>
        /// Iterate over other. If contained in this, mark an element in bit array corresponding to
        /// its position in _slots. If anything is unmarked (in bit array), remove it.
        /// 
        /// This attempts to allocate on the stack, if below StackAllocThreshold.
        /// </summary>
        /// <param name="other"></param>
        protected virtual void IntersectWithEnumerable(IEnumerable<T> other)
        {
            var exist = new HashSet<T>(Comparer);
            // mark if contains: find index of in slots array and mark corresponding element in bit array
            foreach (var item in other)
            {
                if (Contains(item))
                {
                    exist.Add(item);
                }
            }

            foreach (var item in this)
            {
                if (!exist.Contains(item))
                {
                    Remove(item);
                }
            }
        }

        /// <inheritdoc />
        public virtual bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Count == 0 || Equals(other, this)) 
            {
                return false;
            }

            if (other is System.Collections.Generic.ICollection<T> collection)
            {
                if (collection.Count == 0)
                {
                    return true;
                }


                if (other is AbstractSet<T> set && AreComparersEqual(this, set))
                {
                    if (set.Count >= Count)
                    {
                        return false;
                    }

                    return ContainsAllElements(other);
                }
            }

            var (uniqueCount, unfoundCount) = CheckUniqueAndUnfoundElements(other, true);
            return uniqueCount < Count && unfoundCount == 0;
        }

        /// <inheritdoc />
        public virtual bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Count == 0 || Equals(other, this))
            {
                return true;
            }

            if (other is System.Collections.Generic.ICollection<T> collection)
            {
                if (Count > collection.Count)
                {
                    return false;
                }

                if (other is AbstractSet<T> hash && AreComparersEqual(this, hash))
                {
                    return IsSubsetOfHashSetWithSameEc(hash);
                }
            }

            var (uniqueCount, unfoundCount) = CheckUniqueAndUnfoundElements(other, false);
            return uniqueCount == Count && unfoundCount >= 0;
        }

        /// <inheritdoc />
        public virtual bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            // a set is always a superset of itself
            if (Equals(other, this))
            {
                return true;
            }

            // try to fall out early based on counts
            if (other is System.Collections.Generic.ICollection<T> collection)
            {
                // if other is the empty set then this is a superset
                if (collection.Count == 0)
                {
                    return true;
                }

                // try to compare based on counts alone if other is a hashset with
                // same equality comparer
                if (other is AbstractSet<T> otherAsSet 
                    && AreComparersEqual(this, otherAsSet)
                    && otherAsSet.Count > Count)
                {
                    return false;
                }
            }

            return ContainsAllElements(other);
        }

        /// <inheritdoc />
        public virtual bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Count == 0)
            {
                return false;
            }

            // set overlaps itself
            if (Equals(other, this))
            {
                return true;
            }

            foreach (var element in other)
            {
                if (Contains(element))
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <inheritdoc />
        public virtual bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Equals(other, this))
            {
                return true;
            }

            if (other is AbstractSet<T> set && AreComparersEqual(this, set))
            {
                if (Count != set.Count)
                {
                    return false;
                }

                return ContainsAllElements(set);
            }

            if (other is System.Collections.Generic.ICollection<T> collection)
            {
                if (Count == 0 && collection.Count > 0)
                {
                    return false;
                }
            }
            
            var (uniqueCount, unfoundCount) = CheckUniqueAndUnfoundElements(other, true);
            return uniqueCount == Count && unfoundCount == 0;
        }

        /// <inheritdoc />
        public virtual void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Count == 0)
            {
                UnionWith(other);
                return;
            }

            if (Equals(other, this))
            {
                Clear();
                return;
            }

            if (other is AbstractSet<T> set && AreComparersEqual(this, set))
            {
                SymmetricExceptWithUniqueHashSet(set);
            }
            else
            {
                SymmetricExceptWithEnumerable(other);
            }
        }
        
        /// <inheritdoc />
        public virtual void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (var item in other)
            {
                TryAdd(item);
            }
        }
        
        /// <summary>
        /// Checks if this contains of other's elements. Iterates over other's elements and
        /// returns false as soon as it finds an element in other that's not in this.
        /// Used by SupersetOf, ProperSupersetOf, and SetEquals.
        /// </summary>
        /// <param name="other">The <see cref="IEnumerable{T}"/></param>
        /// <returns>Return if contains all elements</returns>
        protected virtual bool ContainsAllElements([NotNull]IEnumerable<T> other)
        {
            foreach (var element in other)
            {
                if (!Contains(element))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Checks if equality comparers are equal. This is used for algorithms that can
        /// speed up if it knows the other item has unique elements. I.e. if they're using
        /// different equality comparers, then uniqueness assumption between sets break.
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        protected static bool AreComparersEqual(AbstractSet<T> set1, AbstractSet<T> set2) => set1.Comparer.Equals(set2.Comparer);

        /// <summary>
        /// Determines counts that can be used to determine equality, subset, and superset. This
        /// is only used when other is an IEnumerable and not a HashSet. If other is a HashSet
        /// these properties can be checked faster without use of marking because we can assume
        /// other has no duplicates.
        ///
        /// The following count checks are performed by callers:
        /// 1. Equals: checks if unfoundCount = 0 and uniqueFoundCount = _count; i.e. everything
        /// in other is in this and everything in this is in other
        /// 2. Subset: checks if unfoundCount >= 0 and uniqueFoundCount = _count; i.e. other may
        /// have elements not in this and everything in this is in other
        /// 3. Proper subset: checks if unfoundCount > 0 and uniqueFoundCount = _count; i.e
        /// other must have at least one element not in this and everything in this is in other
        /// 4. Proper superset: checks if unfound count = 0 and uniqueFoundCount strictly less
        /// than _count; i.e. everything in other was in this and this had at least one element
        /// not contained in other.
        ///
        /// An earlier implementation used delegates to perform these checks rather than returning
        /// an ElementCount struct; however this was changed due to the perf overhead of delegates.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="returnIfUnfound">Allows us to finish faster for equals and proper superset
        /// because unfoundCount must be 0.</param>
        /// <returns></returns>
        protected virtual (int UniqueCount, int UnfoundCount) CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
        {
            // need special case in case this has no elements.
            if (Count == 0)
            {
                var numElementsInOther = 0;
                foreach (var _ in other)
                {
                    numElementsInOther++;
                    // break right away, all we want to know is whether other has 0 or 1 elements
                    break;
                }
                
                return (0, numElementsInOther);
            }
            
            // count of items in other not found in this
            var unfoundCount = 0;
            
            // count of unique items in other found in this
            var uniqueFoundCount = 0;
            
            // to ensure that item hasn't been seen yet
            var hash = new HashSet<T>();

            foreach (var item in other)
            {
                if(Contains(item))
                {
                    if (hash.Add(item))
                    {
                        uniqueFoundCount++;
                    }   
                }
                else
                {
                    unfoundCount++;
                    if (returnIfUnfound)
                    {
                        break;
                    }
                }
            }

            return (uniqueFoundCount, unfoundCount);
        }
        
        /// <summary>
        /// Implementation Notes:
        /// If other is a hashset and is using same equality comparer, then checking subset is
        /// faster. Simply check that each element in this is in other.
        ///
        /// Note: if other doesn't use same equality comparer, then Contains check is invalid,
        /// which is why callers must take are of this.
        ///
        /// If callers are concerned about whether this is a proper subset, they take care of that.
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected virtual bool IsSubsetOfHashSetWithSameEc(AbstractSet<T> other)
        {
            foreach (var item in this)
            {
                if (!other.Contains(item))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// if other is a set, we can assume it doesn't have duplicate elements, so use this
        /// technique: if can't remove, then it wasn't present in this set, so add.
        ///
        /// As with other methods, callers take care of ensuring that other is a hashset using the
        /// same equality comparer.
        /// </summary>
        /// <param name="other"></param>
        protected virtual void SymmetricExceptWithUniqueHashSet(AbstractSet<T> other)
        {
            foreach (var item in other)
            {
                if (!Remove(item))
                {
                    TryAdd(item);
                }
            }
        }
        
        /// <summary>
        /// Implementation notes:
        ///
        /// Used for symmetric except when other isn't a HashSet. This is more tedious because
        /// other may contain duplicates. HashSet technique could fail in these situations:
        /// 1. Other has a duplicate that's not in this: HashSet technique would add then
        /// remove it.
        /// 2. Other has a duplicate that's in this: HashSet technique would remove then add it
        /// back.
        /// In general, its presence would be toggled each time it appears in other.
        ///
        /// This technique uses bit marking to indicate whether to add/remove the item. If already
        /// present in collection, it will get marked for deletion. If added from other, it will
        /// get marked as something not to remove.
        ///
        /// </summary>
        /// <param name="other"></param>
        protected virtual void SymmetricExceptWithEnumerable(IEnumerable<T> other)
        {
            var tmp = new HashSet<T>(Count);
            foreach (var item in other)
            {
                if(tmp.Add(item))
                {
                    if (!TryAdd(item))
                    {
                        Remove(item);
                    }
                }
            }
        }
    }
}
