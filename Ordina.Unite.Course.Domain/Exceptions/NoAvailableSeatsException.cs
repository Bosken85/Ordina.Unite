using System;

namespace Ordina.Unite.Course.Domain.Exceptions
{
    [Serializable]
    public class NoAvailableSeatsException: Exception
    {
        public NoAvailableSeatsException() { }
        public NoAvailableSeatsException(string message) : base(message) { }
        public NoAvailableSeatsException(string message, Exception inner) : base(message, inner) { }
        protected NoAvailableSeatsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
