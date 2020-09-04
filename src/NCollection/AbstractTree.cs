namespace NCollection
{
    /// <summary>
    /// This class provides a skeletal implementation of the <see cref="ITree{T}"/>
    /// interface, to minimize the effort required to implement this interface.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public abstract class AbstractTree<T> : AbstractCollection<T>, ITree<T>
    {
        
    }
}