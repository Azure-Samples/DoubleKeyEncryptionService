using System;
using System.Collections.Generic;

namespace Microsoft.InformationProtection.Web.Models
{
    public class TestKey : IKey
    {        
        private string privateKeyPem;
        private string publicKeyPem;
        private PublicKey storedPublicKey = null;
        System.Security.Cryptography.RSA cryptoEngine = null;

        public TestKey(string publicKey, string privateKey)
        {
            publicKeyPem = publicKey;
            privateKeyPem = privateKey;
        }

        void IntializeCrypto()
        {
            if(cryptoEngine == null)
            {
                var tempCryptoEngine = System.Security.Cryptography.RSA.Create();
                byte[] privateKeyBytes = System.Convert.FromBase64String(privateKeyPem);
                tempCryptoEngine.ImportRSAPrivateKey(privateKeyBytes, out int bytesRead);

                var RSAKeyInfo = tempCryptoEngine.ExportParameters(false);
                var exponent = ByteArrayToUInt(RSAKeyInfo.Exponent);
                var modulus = Convert.ToBase64String(RSAKeyInfo.Modulus);
                storedPublicKey = new PublicKey(modulus, exponent);

                cryptoEngine = tempCryptoEngine;
            }
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
    }
}