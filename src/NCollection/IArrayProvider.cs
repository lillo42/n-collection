namespace NCollection
{
    /// <summary>
    /// Provides a array instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IArrayProvider<T>
    {
        /// <summary>
        /// Retrieves a instance of array <see cref="T"/>. 
        /// </summary>
        /// <param name="minimumLength">The minimum length of the array needed.</param>
        /// <returns>An array that is at least <paramref name="minimumLength"/> in length.</returns>
        T[] GetOrCreate(int minimumLength);
                 
        /// <summary>
        /// Return the allocated array.
        /// </summary>
        /// <param name="values"></param>
        void Return(T[] values);
    }
}
