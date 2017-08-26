using System;

namespace Expo3.Model.Exceptions
{
    public class UpdateEntityException : Exception
    {
        public UpdateEntityException() : base()
        {
        }

        public UpdateEntityException(string message) : base(message)
        {
        }

        public UpdateEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}