using System;
using System.Security.Claims;

namespace Microsoft.InformationProtection.Web.Models
{
    public class KeyManager
    {
        private readonly KeyStore mKeyStore;

        public KeyManager(KeyStore keyStore)
        {
            mKeyStore = keyStore;
        }

        public KeyData GetPublicKey(Uri requestUri, string keyName)
        {
            var key = mKeyStore.GetActiveKey(keyName);
            var publicKey = key.GetKey().GetPublicKey();

            publicKey.kid = requestUri.GetLeftPart(UriPartial.Path) + "/" + key.GetKeyId();
            publicKey.kty = key.GetKeyType();
            publicKey.alg = key.GetSupportedAlgorithm();

            var exp = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss");

            return new KeyData(publicKey, new Cache(exp));
        }

        public DecryptedData Decrypt(ClaimsPrincipal user, string keyName, string keyId, EncryptedData encryptedData)
        {
            var keyData = mKeyStore.GetKey(keyName, keyId);

            keyData.GetKeyAuth().CanUserAccessKey(user, keyData);

            if (encryptedData.alg != "RSA-OAEP-256")
            {
                throw new Exception(encryptedData.alg + " is not supported");
            }
            
            var decryptedData = keyData.GetKey().Decrypt(Convert.FromBase64String(encryptedData.value));

            return new DecryptedData(Convert.ToBase64String(decryptedData));
        } 

    }
}