// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    //The classes in this file implement the format of public key data returned for a key in json format
    //Changing the returned data can break consuming clients
    //See src\customer-key-store\Protocols\PublicKey.Response.json
    public class KeyData
    {
        public KeyData(PublicKey key, PublicKeyCache cache)
        {
            this.Key = key;
            this.Cache = cache;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <remarks>
        /// The key information.
        /// Required.
        /// See PublicKey documentation below.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty("key")]
        public PublicKey Key { get; private set; }

        /// <summary>
        /// Gets the Cache.
        /// </summary>
        /// <remarks>
        /// Details how the public key is cached locally
        /// Optional.
        /// If omitted then caching of the public key is disabled and encryption of content will always require a call to the key store
        /// </remarks>
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

        /// <summary>
        /// Gets or sets the key type.
        /// </summary>
        /// <remarks>
        /// The key type.
        /// Required.
        /// The only supported value is 'RSA'.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty("kty")]
        public string KeyType { get; set; }

        /// <summary>
        /// Gets the public key modulus.
        /// </summary>
        /// <remarks>
        /// The public key modulus in base 64 format.
        /// Required.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty("n")]
        public string Modulus { get; private set; }

        /// <summary>
        /// Gets the key exponent.
        /// </summary>
        /// <remarks>
        /// The public key exponent in base 10 numeric format.
        /// Required.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty("e")]
        public uint Exponent { get; private set; }

        /// <summary>
        /// Gets or sets the algorithm.
        /// </summary>
        /// <remarks>
        /// The supported algorithm that can be used to encrypt the data.
        /// Required.
        /// The only supported value is 'RS256'.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty("alg")]
        public string Algorithm { get; set; }

        /// <summary>
        /// Gets or sets the key id.
        /// </summary>
        /// <remarks>
        /// The key Id.
        /// Required.
        /// A URI that identifies the key that is in use for the key name.  The format is {URI}/{KeyName}/{KeyVersion-Guid}
        /// This URI will be called by the client to decrypt the data by appending /decrypt to the end.
        /// Ex. https://hostname/KeyName/2BE4E378-1317-4D64-AC44-D75f638F7B29
        /// </remarks>
        [Newtonsoft.Json.JsonProperty("kid")]
        public string KeyId { get; set; }
    }

    public class PublicKeyCache
    {
        public PublicKeyCache(string expiration)
        {
            this.Expiration = expiration;
        }

        /// <summary>
        /// Gets the expiration.
        /// </summary>
        /// <remarks>
        /// This member specifies the expiration date and time in format yyyy-MM-ddTHH:mm:ss - after which a locally stored public key will expire and require a call to 
        //  the customer key store to obtain a newer version.
        /// Required.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty("exp")]
        public string Expiration { get; private set; }
    }
}
