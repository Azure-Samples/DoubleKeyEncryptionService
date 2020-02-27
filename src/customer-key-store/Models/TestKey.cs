// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    using System;
    using Microsoft.InformationProtection.Web.Models.Extensions;
    using sg = System.Globalization;
    public class TestKey : IKey
    {
        private string privateKeyPem;
        private string publicKeyPem;
        private PublicKey storedPublicKey = null;
        private System.Security.Cryptography.RSA cryptoEngine = null;

        public TestKey(string publicKey, string privateKey)
        {
            publicKeyPem = publicKey;
            privateKeyPem = privateKey;
        }

        public PublicKey GetPublicKey()
        {
            IntializeCrypto();

            return storedPublicKey;
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            IntializeCrypto();

            return cryptoEngine.Decrypt(encryptedData, System.Security.Cryptography.RSAEncryptionPadding.OaepSHA256);
        }

        private static uint ByteArrayToUInt(byte[] array)
        {
            uint retVal = 0;

            checked
            {
              if (BitConverter.IsLittleEndian)
              {
                  for (int index = array.Length - 1; index >= 0; index--)
                  {
                      retVal = (retVal << 8) + array[index];
                  }
              }
              else
              {
                  for (int index = 0; index < array.Length; index++)
                  {
                      retVal = (retVal << 8) + array[index];
                  }
              }
            }

            return retVal;
        }

        private void IntializeCrypto()
        {
            if(cryptoEngine == null)
            {
                var tempCryptoEngine = System.Security.Cryptography.RSA.Create();
                byte[] privateKeyBytes = System.Convert.FromBase64String(privateKeyPem);
                tempCryptoEngine.ImportRSAPrivateKey(privateKeyBytes, out int bytesRead);

                var rsaKeyInfo = tempCryptoEngine.ExportParameters(false);
                var exponent = ByteArrayToUInt(rsaKeyInfo.Exponent);
                var modulus = Convert.ToBase64String(rsaKeyInfo.Modulus);
                storedPublicKey = new PublicKey(modulus, exponent);

                cryptoEngine = tempCryptoEngine;
            }
        }
    }
}