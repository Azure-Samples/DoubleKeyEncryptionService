using System;

namespace Microsoft.InformationProtection.Web.Models
{
    public class TestKey : Key
    {
        private static readonly string kPublicKey =
@"-----BEGIN PUBLIC KEY-----
{0}
-----END PUBLIC KEY-----";

        private static readonly string kPrivateKey = 
@"-----BEGIN RSA PRIVATE KEY-----
{0}
-----END RSA PRIVATE KEY-----";

        private string mPublicKey;
        private string mPrivateKey;

        public TestKey(string publicKey, string privateKey)
        {
            mPublicKey = String.Format(kPublicKey, publicKey);
            mPrivateKey = String.Format(kPrivateKey, privateKey);
        }
        
        public PublicKey GetPublicKey()
        {
            Org.BouncyCastle.OpenSsl.PemReader pr = 
                new Org.BouncyCastle.OpenSsl.PemReader(
                        new System.IO.StringReader(mPublicKey));

            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters KeyParams 
                = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)pr.ReadObject();

            var RSAKeyInfo = Org.BouncyCastle.Security.DotNetUtilities.ToRSAParameters(KeyParams);
            var exponent = ByteArrayToUInt(RSAKeyInfo.Exponent);
            var modulus = Convert.ToBase64String(RSAKeyInfo.Modulus);

            return new PublicKey(modulus, exponent);
        }
        
        public byte[] Decrypt(byte[] encryptedData)
        {
            var encryptEngine = new Org.BouncyCastle.Crypto.Encodings.OaepEncoding(new Org.BouncyCastle.Crypto.Engines.RsaEngine(), new Org.BouncyCastle.Crypto.Digests.Sha256Digest());

            Org.BouncyCastle.OpenSsl.PemReader pr =
                new Org.BouncyCastle.OpenSsl.PemReader(
                        new System.IO.StringReader(mPrivateKey));
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair KeyParamsprivate
                = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)pr.ReadObject();

            encryptEngine.Init(false, KeyParamsprivate.Private);

            return encryptEngine.ProcessBlock(encryptedData, 0, encryptedData.Length);
        }

        private static uint ByteArrayToUInt(byte[] array)
        {
            uint retVal = 0;

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

            return retVal;
        }        
    }
}