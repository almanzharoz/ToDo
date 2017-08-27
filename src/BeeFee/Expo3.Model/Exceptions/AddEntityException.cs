﻿using System;

namespace Expo3.Model.Exceptions
{
    public class AddEntityException : Exception
    {
        public AddEntityException() : base()
        {
        }

        public AddEntityException(string message) : base(message)
        {
        }

        public AddEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}