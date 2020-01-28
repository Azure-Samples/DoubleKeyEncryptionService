// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    using System;
    using System.Security.Claims;
    using Microsoft.InformationProtection.Web.Models.Extensions;
    using sg = System.Globalization;

    public class KeyManager
    {
        private readonly IKeyStore keyStore;

        public KeyManager(IKeyStore keyStore)
        {
            this.keyStore = keyStore;
        }

        public KeyData GetPublicKey(Uri requestUri, string keyName)
        {
            requestUri.ThrowIfNull(nameof(requestUri));
            keyName.ThrowIfNull(nameof(keyName));

            var key = keyStore.GetActiveKey(keyName);
            var publicKey = key.Key.GetPublicKey();

            publicKey.KeyId = requestUri.GetLeftPart(UriPartial.Path) + "/" + key.KeyId;
            publicKey.KeyType = key.KeyType;
            publicKey.Algorithm = key.SupportedAlgorithm;
            PublicKeyCache cache = null;

            if(key.ExpirationTimeInDays.HasValue)
            {
                cache = new PublicKeyCache(
                    DateTime.UtcNow.AddDays(
                        key.ExpirationTimeInDays.Value).ToString("yyyy-MM-ddTHH:mm:ss", sg.CultureInfo.InvariantCulture));
            }

            return new KeyData(publicKey, cache);
        }

        public DecryptedData Decrypt(ClaimsPrincipal user, string keyName, string keyId, EncryptedData encryptedData)
        {
            user.ThrowIfNull(nameof(user));
            keyName.ThrowIfNull(nameof(keyName));
            keyId.ThrowIfNull(nameof(keyId));
            encryptedData.ThrowIfNull(nameof(encryptedData));

            var keyData = keyStore.GetKey(keyName, keyId);

            keyData.KeyAuth.CanUserAccessKey(user, keyData);

            if (encryptedData.Algorithm != "RSA-OAEP-256")
            {
                throw new ArgumentException(encryptedData.Algorithm + " is not supported");
            }

            var decryptedData = keyData.Key.Decrypt(Convert.FromBase64String(encryptedData.Value));
            return new DecryptedData(Convert.ToBase64String(decryptedData));
        }
    }
}