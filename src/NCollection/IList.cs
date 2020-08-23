using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NCollection
{
    public interface IList<T> : ICollection<T>, System.Collections.Generic.IList<T>
    {
        /// <summary>
        ///  Inserts all of the elements in the specified collection into this
        /// list at the specified position (optional operation).  Shifts the
        /// element currently at that position (if any) and any subsequent
        /// elements to the right (increases their indices).  The new elements
        /// will appear in this list in the order that they are returned by the
        /// specified collection's iterator.  The behavior of this operation is
        /// undefined if the specified collection is modified while the
        /// operation is in progress.  (Note that this will occur if the specified
        /// collection is this list, and it's nonempty.)
        /// </summary>
        /// <param name="index">index at which to insert the first element from the specified collection</param>
        /// <param name="source">The source containing elements to be added to this list</param>
        /// <returns>true if this list changed as a result of the call</returns>
        /// <exception cref="ArgumentException">if some property of an element of the specified collection prevents it from being added to this list</exception>
        /// <exception cref="ArgumentNullException">if <see cref="source"/> is null</exception>
        /// <exception cref="NullReferenceException">if the specified collection contains one or more null elements and this list does not permit null elements, or if the specified collection is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range
        /// <see cref="index"/> < 0 || <see cref="index"/> > <see cref="ICollection{T}.Count"/>
        /// </exception>
        bool AddRange(int index, [NotNull]IEnumerable<T> source);

        /// <summary>
        /// Returns a view of the portion of this list between the specified
        /// <see cref="fromIndex"/>, inclusive, and <see cref="toIndex"/>, exclusive.  (If
        /// <see cref="fromIndex"/> and <see cref="toIndex"/> are equal, the returned list is
        /// empty.)  The returned list is backed by this list, so non-structural
        /// changes in the returned list are reflected in this list, and vice-versa.
        /// The returned list supports all of the optional list operations supported
        /// by this list.
        /// 
        /// This method eliminates the need for explicit range operations (of
        /// the sort that commonly exist for arrays).  Any operation that expects
        /// a list can be used as a range operation by passing a subList view
        /// instead of a whole list.  For example, the following idiom
        /// removes a range of elements from a list:
        /// <code>
        ///     list.SubList(from, to).clear();
        /// </code>
        /// Similar idioms may be constructed for <see cref="System.Collections.Generic.IList{T}.IndexOf"/> and
        /// {@code lastIndexOf}, and all of the algorithms in the
        /// <see cref="ICollection{T}"/> class can be applied to a subList.
        /// The semantics of the list returned by this method become undefined if
        /// the backing list (i.e., this list) is structurally modified  in
        /// any way other than via the returned list.  (Structural modifications are
        /// those that change the size of this list, or otherwise perturb it in such
        /// a fashion that iterations in progress may yield incorrect results.)
        /// </summary>
        /// <param name="fromIndex">low endpoint (inclusive) of the subList</param>
        /// <param name="toIndex">high endpoint (exclusive) of the subList</param>
        /// <returns>A view of the specified range within this list</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range
        /// <see cref="index"/> < 0 || <see cref="index"/> > <see cref="ICollection{T}.Count"/>
        /// </exception>
        IList<T> SubList(int fromIndex, int toIndex);

        /// <summary>
        /// Returns the index of the last occurrence of the specified element
        /// in this list, or -1 if this list does not contain the element.
        /// More formally, returns the highest index  i such that
        ///  object.equals(o, this[i]),
        /// or -1 if there is no such index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int LastIndexOf(T item);
    }
}