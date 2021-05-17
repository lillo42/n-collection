namespace NCollection
{
    /// <summary>
    /// The implementation of <see cref="IArrayProvider{T}"/> using Adhoc array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayProvider<T> : IArrayProvider<T>
    {
        /// <summary>
        /// The static instance of <see cref="ArrayProvider{T}"/>. 
        /// </summary>
        public static ArrayProvider<T> Instance { get; } = new();
        
        /// <summary>
        /// Create new instance of array with length <paramref name="minimumLength"/>.
        /// </summary>
        /// <param name="minimumLength">The length of the array needed.</param>
        /// <returns>An array with <paramref name="minimumLength"/> length.</returns>
        public T[] GetOrCreate(int minimumLength) => new T[minimumLength];

        
        void IArrayProvider<T>.Return(T[] values)
        {
            
        }
    }
}
