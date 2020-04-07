namespace NCollection
{
    public interface ICollection : System.Collections.ICollection
    {
        bool IsReadOnly { get; }
        
        void Add(object? item);

        void Clear();
        
        bool Contains(object? item);

        void Remove(object? item);
        
    }
}
