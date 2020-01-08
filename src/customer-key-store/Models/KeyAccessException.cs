// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace CustomerKeyStore.Models
{
    using System;
    public class KeyAccessException : Exception
    {
        public KeyAccessException(string message)
            : base(message)
        {
        }

        public KeyAccessException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private KeyAccessException()
        {
        }
    }
}
