using System.Buffers;

namespace NCollection
{
    /// <summary>
    /// The implementation of <see cref="IArrayProvider{T}"/> using <see cref="ArrayPool{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayPoolProvider<T> : IArrayProvider<T>
    {
        private readonly ArrayPool<T> _pool;

        /// <summary>
        /// The instance <see cref="ArrayPoolProvider{T}"/>.
        /// </summary>
        public static ArrayPoolProvider<T> Instance { get; } = new();

        /// <summary>
        /// Initialize <see cref="ArrayPoolProvider{T}"/> with share instance of <see cref="ArrayPool{T}"/>.
        /// </summary>
        public ArrayPoolProvider()
        {
            _pool = ArrayPool<T>.Shared;
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayPoolProvider{T}"/> with provider instance of <see cref="ArrayPool{T}"/>.
        /// </summary>
        /// <param name="pool">The instance of <see cref="ArrayPool{T}"/>.</param>
        public ArrayPoolProvider(ArrayPool<T> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// Retrieves a buffer that is at least the requested length.
        /// </summary>
        /// <param name="minimumLength">The minimum length of the array needed.</param>
        /// <returns>An array that is at least <paramref name="minimumLength"/> in length.</returns>
        public T[] GetOrCreate(int minimumLength) => _pool.Rent(minimumLength);

        /// <summary>
        /// Returns to the pool an array that was previously obtained via <see cref="GetOrCreate"/> on the same <see cref="ArrayPoolProvider{T}"/> instance.
        /// </summary>
        /// <param name="values">he buffer previously obtained from <see cref="GetOrCreate"/> to return to the pool.</param>
        public void Return(T[] values) =>_pool.Return(values, true);
    }
}
