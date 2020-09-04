using System;

namespace NCollection
{
    /// <summary>
    /// A collection that contains no duplicate elements.  More formally, sets
    /// contain no pair of elements e1 and  e2 such that
    /// e1.Equals(e2), and at most one null element.  As implied by
    /// its name, this interface models the mathematical set abstraction.
    ///
    /// The <see cref="ISet{T}"/> interface places additional stipulations, beyond those
    /// inherited from the <see cref="ICollection{T}"/> interface, on the contracts of all
    /// constructors and on the contracts of the <see cref="Add"/> methods.
    /// Declarations for other inherited methods are
    /// also included here for convenience.  (The specifications accompanying these
    /// declarations have been tailored to the <see cref="ISet{T}"/> interface, but they do
    /// not contain any additional stipulations.)
    ///
    /// The additional stipulation on constructors is, not surprisingly,
    /// that all constructors must create a set that contains no duplicate elements
    /// (as defined above).
    ///
    /// Note: Great care must be exercised if mutable objects are used as set
    /// elements.  The behavior of a set is not specified if the value of an object
    /// is changed in a manner that affects {@code equals} comparisons while the
    /// object is an element in the set.  A special case of this prohibition is
    /// that it is not permissible for a set to contain itself as an element.
    ///
    /// Some set implementations have restrictions on the elements that
    /// they may contain.  For example, some implementations prohibit null elements,
    /// and some have restrictions on the types of their elements.  Attempting
    /// to query the presence of an ineligible element may throw an exception,
    /// or it may simply return false; some implementations will exhibit the former
    /// behavior and some will exhibit the latter.  More generally, attempting an
    /// operation on an ineligible element whose completion would not result in
    /// the insertion of an ineligible element into the set may throw an
    /// exception or it may succeed, at the option of the implementation.
    /// Such exceptions are marked as "optional" in the specification for this
    /// interface.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public interface ISet<T> : ICollection<T>, System.Collections.Generic.ISet<T>
    {
        /// <summary>
        ///  Ensures that this collection contains the specified element (optional
        /// operation). Returns true if this collection changed as a
        /// result of the call.  (Returns false if this collection does
        /// not permit duplicates and already contains the specified element.)
        ///
        /// Collections that support this operation may place limitations on what
        /// elements may be added to this collection.  In particular, some
        /// collections will refuse to add null elements, and others will
        /// impose restrictions on the type of elements that may be added.
        /// Collection classes should clearly specify in their documentation any
        /// restrictions on what elements may be added.
        ///
        /// If a collection refuses to add a particular element for any reason
        /// other than that it already contains the element, it must throw
        /// an exception (rather than returning false).  This preserves
        /// the invariant that a collection always contains the specified element
        /// after this call returns.
        /// </summary>
        /// <param name="item">Element whose presence in this collection is to be ensured</param>
        /// <returns>True if this collection changed as a result of the call</returns>
        /// <exception cref="ArgumentNullException">f the specified element is null and this collection does not permit null elements</exception>
        /// <exception cref="ArgumentException">if some property of the element prevents it from being added to this collection</exception>
        /// <exception cref="InvalidOperationException">if the element cannot be added at this time due to insertion restrictions</exception>
        new bool Add(T item);

        #region ICollection

        void System.Collections.Generic.ICollection<T>.Add(T item) => Add(item);

        #endregion

        #region Set

        bool System.Collections.Generic.ISet<T>.Add(T item) => Add(item);

        #endregion
    }
}