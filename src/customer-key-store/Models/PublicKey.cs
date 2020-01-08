// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    //The classes in this file implement the format of public key data returned for a key
    //Changing the returned data can break consuming clients
    public class KeyData
    {
        public KeyData(PublicKey key, PublicKeyCache cache)
        {
            this.Key = key;
            this.Cache = cache;
        }

        [Newtonsoft.Json.JsonProperty("key")]
        public PublicKey Key { get; private set; }

        [Newtonsoft.Json.JsonProperty("cache", NullValueHandling=Newtonsoft.Json.NullValueHandling.Ignore)]
        public PublicKeyCache Cache { get; private set; }
    }

    public class PublicKey
    {
        public PublicKey(string modulus, uint exponent)
        {
            this.KeyType = string.Empty;
            this.Modulus = modulus;
            this.Exponent = exponent;
            this.Algorithm = string.Empty;
            this.KeyId = string.Empty;
        }

        [Newtonsoft.Json.JsonProperty("kty")]
        public string KeyType { get; set; }
        [Newtonsoft.Json.JsonProperty("n")]
        public string Modulus { get; private set; }
        [Newtonsoft.Json.JsonProperty("e")]
        public uint Exponent { get; private set; }
        [Newtonsoft.Json.JsonProperty("alg")]
        public string Algorithm { get; set; }
        [Newtonsoft.Json.JsonProperty("kid")]
        public string KeyId { get; set; }
    }

    public class PublicKeyCache
    {
        public PublicKeyCache(string expiration)
        {
            this.Expiration = expiration;
        }

        [Newtonsoft.Json.JsonProperty("exp")]
        public string Expiration { get; private set; }
    }
}
