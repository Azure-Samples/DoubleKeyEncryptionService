// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    public class KeyStoreData
    {
        private IKey key;
        private string keyId;
        private string keyType;
        private string supportedAlgorithm;
        private IAuthorizer keyAuth;
        private int? expirationTimeInDays;

        public KeyStoreData(IKey key, string keyId, string keyType, string supportedAlgorithm, IAuthorizer keyAuth, int? expirationTimeInDays)
        {
            this.key = key;
            this.keyId = keyId;
            this.keyType = keyType;
            this.supportedAlgorithm = supportedAlgorithm;
            this.keyAuth = keyAuth;
            this.expirationTimeInDays = expirationTimeInDays;
        }

        public IKey Key { get { return key; } }
        public string KeyId { get { return keyId; } }
        public string KeyType { get { return keyType; } }
        public string SupportedAlgorithm { get { return supportedAlgorithm; } }
        public IAuthorizer KeyAuth { get { return keyAuth; } }
        public int? ExpirationTimeInDays { get { return expirationTimeInDays; } }
    }
}
