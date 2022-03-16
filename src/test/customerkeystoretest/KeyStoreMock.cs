// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Customerkeystoretest
{
    using System;
    using System.Collections.Generic;
    using Microsoft.InformationProtection.Web.Models;

    public class KeyStoreMock : IKeyStore
    {
        private Dictionary<string, Dictionary<string, KeyStoreData>> keys = new Dictionary<string, Dictionary<string, KeyStoreData>>();
        private Dictionary<string, string> activeKeys = new Dictionary<string, string>();

        public void AddKey(bool active, string keyName, string keyId, IKey key, string keyType, string supportedAlgorithm, IAuthorizer keyAuth, int? expirationTimeInDays)
        {
            if(!keys.ContainsKey(keyName))
            {
                keys[keyName] = new Dictionary<string, KeyStoreData>();
            }
            keys[keyName][keyId] = new KeyStoreData(key, keyId, keyType, supportedAlgorithm, keyAuth, expirationTimeInDays);
            if(active)
            {
                activeKeys[keyName] = keyId;
            }
        }

        public KeyStoreData GetActiveKey(string keyName)
        {
            Dictionary<string, KeyStoreData> keys;
            string activeKey;
            KeyStoreData foundKey;
            if(!this.keys.TryGetValue(keyName, out keys) || !activeKeys.TryGetValue(keyName, out activeKey) ||
                    !keys.TryGetValue(activeKey, out foundKey))
            {
                throw new ArgumentException("Key " + keyName + " not found");
            }

            return foundKey;
        }

        public KeyStoreData GetKey(string keyName, string keyId)
        {
            Dictionary<string, KeyStoreData> keys;
            KeyStoreData foundKey;
            if(!this.keys.TryGetValue(keyName, out keys) ||
                    !keys.TryGetValue(keyId, out foundKey))
            {
                throw new ArgumentException("Key " + keyName + "-" + keyId + " not found");
            }

            return foundKey;
        }
    }
}
