using System;
using System.Collections;
using System.Collections.Generic;

namespace NCollection.Generics
{
    public interface ISortAlgorithm<T> : ISortAlgorithm
    {
        void Execute(T[] array, IComparer<T> comparer);

        void ISortAlgorithm.Execute(object[] array, IComparer comparer)
        {
            var newArray = new T[array.Length];
            Execute(newArray, (IComparer<T>)comparer);
            Array.Copy(newArray, array, array.Length);
        }
    }
}