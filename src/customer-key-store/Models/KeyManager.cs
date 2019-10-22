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
            var publicKey = key.Key.GetPublicKey();

            publicKey.kid = requestUri.GetLeftPart(UriPartial.Path) + "/" + key.KeyId;
            publicKey.kty = key.KeyType;
            publicKey.alg = key.SupportedAlgorithm;

            var exp = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss");

            return new KeyData(publicKey, new Cache(exp));
        }

        public DecryptedData Decrypt(ClaimsPrincipal user, string keyName, string keyId, EncryptedData encryptedData)
        {
            var keyData = mKeyStore.GetKey(keyName, keyId);

            keyData.KeyAuth.CanUserAccessKey(user, keyData);

            if (encryptedData.alg != "RSA-OAEP-256")
            {
                throw new Exception(encryptedData.alg + " is not supported");
            }
            
            var decryptedData = keyData.Key.Decrypt(Convert.FromBase64String(encryptedData.value));

            return new DecryptedData(Convert.ToBase64String(decryptedData));
        } 

    }
}