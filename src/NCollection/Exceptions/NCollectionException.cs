using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NCollection.Exceptions
{
    /// <summary>
    /// Exception base for NCollection
    /// </summary>
    public abstract class NCollectionException : Exception
    {
        protected NCollectionException()
        {
        }

        protected NCollectionException([NotNull] SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        protected NCollectionException(string message) 
            : base(message)
        {
        }

        protected NCollectionException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}