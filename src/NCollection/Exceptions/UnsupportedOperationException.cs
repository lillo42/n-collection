using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NCollection.Exceptions
{
    /// <summary>
    /// Exception to indicate that the requested operation is not supported.
    /// </summary>
    public class UnsupportedOperationException : Exception
    {
        /// <summary>
        ///  Constructs an UnsupportedOperationException with no detail message.
        /// </summary>
        public UnsupportedOperationException()
        {
        }

        
        protected UnsupportedOperationException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Constructs an UnsupportedOperationException with the specified detail message.
        /// </summary>
        /// <param name="message">The detail message</param>
        public UnsupportedOperationException(string message) 
            : base(message)
        {
        }

        
        /// <summary>
        /// Constructs a new exception with the specified detail message and cause.
        /// </summary>
        /// <param name="message">The detail message</param>
        /// <param name="innerException">The detail message</param>
        public UnsupportedOperationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}