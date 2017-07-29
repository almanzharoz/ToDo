using System;

namespace Expo3.Model.Exceptions
{
    public class RemoveEntityException : Exception
    {
        public RemoveEntityException() : base()
        {
        }

        public RemoveEntityException(string message) : base(message)
        {
        }

        public RemoveEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}