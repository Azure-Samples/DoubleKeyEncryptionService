using System;
using sg = System.Globalization;
using Microsoft.InformationProtection.Web.Models.Extensions;

namespace Microsoft.InformationProtection.Web.Models
{
    public class TestKey : IKey
    {
        private const string PublicKeyFormatter =
@"-----BEGIN PUBLIC KEY-----
{0}
-----END PUBLIC KEY-----";

        private const string PrivateKeyFormatter = 
@"-----BEGIN RSA PRIVATE KEY-----
{0}
-----END RSA PRIVATE KEY-----";

        private string publicKey;
        private string privateKey;
        private PublicKey storedPublicKey = null;
        Org.BouncyCastle.Crypto.Encodings.OaepEncoding encryptEngine = null;

        public TestKey(string publicKey, string privateKey)
        {
            this.publicKey = string.Format(sg.CultureInfo.InvariantCulture, PublicKeyFormatter, publicKey);
            this.privateKey = string.Format(sg.CultureInfo.InvariantCulture, PrivateKeyFormatter, privateKey);
        }
        
        public PublicKey GetPublicKey()
        {
            if(storedPublicKey == null) 
            {
                Org.BouncyCastle.OpenSsl.PemReader pr = 
                    new Org.BouncyCastle.OpenSsl.PemReader(
                            new System.IO.StringReader(publicKey));

                Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters KeyParams 
                    = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)pr.ReadObject();

                var RSAKeyInfo = Org.BouncyCastle.Security.DotNetUtilities.ToRSAParameters(KeyParams);
                var exponent = ByteArrayToUInt(RSAKeyInfo.Exponent);
                var modulus = Convert.ToBase64String(RSAKeyInfo.Modulus);
                storedPublicKey = new PublicKey(modulus, exponent);
            }

            return storedPublicKey;
        }
        
        public byte[] Decrypt(byte[] encryptedData)
        {
            encryptedData.ThrowIfNull(nameof(encryptedData));

            if(encryptEngine == null)
            {
              var encryptEngineTemp = new Org.BouncyCastle.Crypto.Encodings.OaepEncoding(new Org.BouncyCastle.Crypto.Engines.RsaEngine(), new Org.BouncyCastle.Crypto.Digests.Sha256Digest());

              Org.BouncyCastle.OpenSsl.PemReader pr =
                  new Org.BouncyCastle.OpenSsl.PemReader(
                          new System.IO.StringReader(privateKey));
              Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair KeyParamsprivate
                  = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)pr.ReadObject();

              encryptEngineTemp.Init(false, KeyParamsprivate.Private);
              encryptEngine = encryptEngineTemp;
            }

            return encryptEngine.ProcessBlock(encryptedData, 0, encryptedData.Length);
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