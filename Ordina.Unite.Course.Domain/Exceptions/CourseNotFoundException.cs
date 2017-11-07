using System;

namespace Ordina.Unite.Course.Domain.Exceptions
{
    [Serializable]
    public class CourseNotFoundException : Exception
    {
        public CourseNotFoundException() : base()
        {
        }

        protected CourseNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}