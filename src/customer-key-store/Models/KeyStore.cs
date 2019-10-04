using System;

namespace Microsoft.InformationProtection.Web.Models
{
    public interface KeyStore
    {
        KeyStoreData GetActiveKey(string keyName);
        KeyStoreData GetKey(string keyName, string keyId);
    }
}