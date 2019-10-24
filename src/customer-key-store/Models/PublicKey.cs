using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.InformationProtection.Web.Models
{
    //The classes in this file implement the format of public key data returned for a key
    //Changing the returned data can break consuming clients
    public class KeyData
    {
        public KeyData(PublicKey key, Cache cache)
        {
            this.Key = key;
            this.Cache = cache;
        }

        [Newtonsoft.Json.JsonProperty("key")]
        public PublicKey Key { get; private set; }

        [Newtonsoft.Json.JsonProperty("cache", NullValueHandling=Newtonsoft.Json.NullValueHandling.Ignore)]
        public Cache Cache { get; private set; }
    }

    public class PublicKey
    {
        public PublicKey(string Modulus, uint Exponent)
        {
            this.KeyType = String.Empty;
            this.Modulus = Modulus;
            this.Exponent = Exponent;
            this.Algorithm = String.Empty;
            this.KeyId = String.Empty;
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

    public class Cache
    {
        public Cache(string Expiration)
        {
            this.Expiration = Expiration;
        }

        [Newtonsoft.Json.JsonProperty("exp")]
        public string Expiration { get; private set; }
    }
}
