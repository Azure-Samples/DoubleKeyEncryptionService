using System;

namespace CustomerKeyStore.Models
{
    public class KeyAccessException : Exception
    {
        private KeyAccessException()
        {

        }
        
        public KeyAccessException(string message)
            : base(message)
        {
        }

        public KeyAccessException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
