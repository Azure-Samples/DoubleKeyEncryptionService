// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Customerkeystoretest
{
    using Microsoft.InformationProtection.Web.Models;

    public class KeyMock : IKey
    {
        private string modulus;
        private uint exponent;

        public KeyMock(string modulus, uint exponent)
        {
            this.modulus = modulus;
            this.exponent = exponent;
        }

        public PublicKey GetPublicKey()
        {
            return new PublicKey(modulus, exponent);
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            return encryptedData;
        }
    }
}
