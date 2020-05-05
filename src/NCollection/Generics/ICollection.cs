namespace NCollection.Generics
{
    public interface ICollection<T> : System.Collections.Generic.ICollection<T>
    {
        bool IsEmpty { get; }
    }
}