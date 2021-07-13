using System;
using System.Runtime.Serialization;

namespace Movies.Client
{
    [Serializable]
    public class ResourseNotFoundException : Exception
    {
        public ResourseNotFoundException()
        {
        }

        public ResourseNotFoundException(string message) : base(message)
        {
        }

        public ResourseNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ResourseNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}