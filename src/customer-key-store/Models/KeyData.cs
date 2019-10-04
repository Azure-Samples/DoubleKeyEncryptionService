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

        public Key GetKey() { return mKey; }
        public string GetKeyId() { return mKeyId; }
        public string GetKeyType() { return mKeyType; }
        public string GetSupportedAlgorithm() { return mSupportedAlgorithm; }
        public Authorizer GetKeyAuth() { return mKeyAuth; }

    }
}