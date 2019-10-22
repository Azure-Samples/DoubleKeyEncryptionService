using System;

namespace Microsoft.InformationProtection.Web.Models
{
    public class KeyStoreData
    {
        private Key mKey;
        private string mKeyId;
        private string mKeyType;
        private string mSupportedAlgorithm;
        private Authorizer mKeyAuth;

        public KeyStoreData(Key key, string keyId, string keyType, string supportedAlgorithm, Authorizer keyAuth)
        {
            mKey = key;
            mKeyId = keyId;
            mKeyType = keyType;
            mSupportedAlgorithm = supportedAlgorithm;
            mKeyAuth = keyAuth;
        }

        public Key Key { get { return mKey; } }
        public string KeyId { get { return mKeyId; } }
        public string KeyType { get { return mKeyType; } }
        public string SupportedAlgorithm { get { return mSupportedAlgorithm; } }
        public Authorizer KeyAuth { get { return mKeyAuth; } }

    }
}
