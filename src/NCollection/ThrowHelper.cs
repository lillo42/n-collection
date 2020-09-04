using System;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    internal static class ThrowHelper
    {
        [DoesNotReturn]
        public static void ThrowWrongValueTypeArgumentException<T>(T value, Type targetType)
        {
            throw GetWrongValueTypeArgumentException(value, targetType);
        }
        
        private static ArgumentException GetWrongValueTypeArgumentException(object? value, Type targetType)
        {
            return new ArgumentException($"Expected value: {targetType}", nameof(value));
        }
    }
}