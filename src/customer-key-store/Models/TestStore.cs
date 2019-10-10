using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;

namespace Microsoft.InformationProtection.Web.Models
{
    public class TestKeyStore : KeyStore
    {

        private Dictionary<string, Dictionary<string, KeyStoreData>> mKeys = new Dictionary<string, Dictionary<string, KeyStoreData>>();
        private Dictionary<string, string> mActiveKeys = new Dictionary<string, string>();
        private IConfiguration mConfiguration;

        private const string kKeyType = "RSA";
        private const string kAlgorithm = "RS256";

        private void CreateTestKey(
            string keyName, 
            string keyId, 
            string publicKey,
            string privateKey,
            string keyType,
            string algorithm,
            Authorizer keyAuth)
        {
            if(keyAuth == null)
            {
                throw new Exception("An authorizer must be specified for a key");
            }

            mKeys.Add(keyName, new Dictionary<string, KeyStoreData>());

            mKeys[keyName][keyId] = new KeyStoreData(
                                                new TestKey(publicKey, privateKey),
                                                keyId,
                                                keyType,
                                                algorithm,
                                                keyAuth);

            if(!mActiveKeys.ContainsKey(keyName))   //Multiple keys with the same name can be in the app settings, the first one for the current name is active, the rest have been rolled
            {
                mActiveKeys[keyName] = keyId;
            }
        }

        public TestKeyStore(IConfiguration configuration)
        {
            mConfiguration = configuration;
            var testKeysSection = mConfiguration.GetSection("TestKeys");
            Authorizer keyAuth = null;
            
            foreach(var testKey in testKeysSection.GetChildren())
            {
                List<string> roles = new List<string>();
                var validRoles = testKey.GetSection("AuthorizedRoles");
                var validEmails = testKey.GetSection("AuthorizedEmailAddress");
                if(validRoles != null && validRoles.Exists())
                {
                    RoleAuthorizer roleAuth = new RoleAuthorizer(mConfiguration);
                    keyAuth = roleAuth;
                    foreach(var role in validRoles.GetChildren())
                    {
                        roleAuth.AddRole(role.Value);
                    }
                }
                else if(validEmails != null && validEmails.Exists())
                {
                    EmailAuthorizer emailAuth = new EmailAuthorizer();
                    keyAuth = emailAuth;
                    foreach(var email in validEmails.GetChildren())
                    {
                        emailAuth.AddEmail(email.Value);
                    }                    
                }

                CreateTestKey(
                    testKey["Name"], 
                    testKey["Id"], 
                    testKey["PublicPem"], 
                    testKey["PrivatePem"], 
                    kKeyType, 
                    kAlgorithm, 
                    keyAuth);
            }
        }

        public KeyStoreData GetActiveKey(string keyName)
        {
            Dictionary<string, KeyStoreData> keys;
            string activeKey;
            KeyStoreData foundKey;
            if(!mKeys.TryGetValue(keyName, out keys) || !mActiveKeys.TryGetValue(keyName, out activeKey) ||
                    !keys.TryGetValue(activeKey, out foundKey))
            {
                throw new Exception("Key " + keyName + " not found");
            }

            return foundKey;
        }

        public KeyStoreData GetKey(string keyName, string keyId)
        {
            Dictionary<string, KeyStoreData> keys;
            KeyStoreData foundKey;
            if(!mKeys.TryGetValue(keyName, out keys) ||
                    !keys.TryGetValue(keyId, out foundKey))
            {
                throw new Exception("Key " + keyName + "-" + keyId + " not found");
            }

            return foundKey;
        }
    }
}