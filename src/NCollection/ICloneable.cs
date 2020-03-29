using System;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    public interface ICloneable<T> : ICloneable
    {
        [return: MaybeNull]
        new T Clone();

        object? ICloneable.Clone()
            => Clone();
    }
}