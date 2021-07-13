using System;
using System.Runtime.Serialization;

namespace Movies.Client
{
    [Serializable]
    public  class UnauthorisedRequestException : Exception
    {
        public UnauthorisedRequestException()
        {
        }

        public UnauthorisedRequestException(string message) : base(message)
        {
        }

        public UnauthorisedRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnauthorisedRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}