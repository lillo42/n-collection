using System.Collections.Generic;

namespace NCollection
{
    /// <summary>
    /// This class provides a skeletal implementation of the <see cref="ITree{T}"/>
    /// interface, to minimize the effort required to implement this interface.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public abstract class AbstractTree<T> : AbstractCollection<T>, ITree<T>
    {
        
        /// <summary>
        /// Returns an enumerator that iterates through the collection using Inorder Traversal.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public abstract IEnumerator<T> InorderTraversal();
        
        /// <summary>
        /// Returns an enumerator that iterates through the collection using Preorder Traversal.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public abstract IEnumerator<T> PreorderTraversal();
        
        /// <summary>
        /// Returns an enumerator that iterates through the collection using Postorder Traversal.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public abstract IEnumerator<T> PostorderTraversal();

        /// <summary>
        /// Returns an enumerator that iterates through the collection using Inorder Traversal.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public override IEnumerator<T> GetEnumerator()
        {
            return InorderTraversal();
        }
    }
}