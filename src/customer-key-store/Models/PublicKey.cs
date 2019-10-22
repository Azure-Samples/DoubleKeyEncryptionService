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
            this.key = key;
            this.cache = cache;
        }

        public PublicKey key { get; set; }

        public Cache cache { get; set; }
    }

    public class PublicKey
    {
        public PublicKey(string kty, string n, uint e, string alg, string kid)
        {
            this.kty = kty;
            this.n = n;
            this.e = e;
            this.alg = alg;
            this.kid = kid;
        }

        public PublicKey(string n, uint e)
        {
            this.kty = String.Empty;
            this.n = n;
            this.e = e;
            this.alg = String.Empty;
            this.kid = String.Empty;
        }        

        public string kty { get; set; }
        public string n { get; set; }
        public uint e { get; set; }
        public string alg { get; set; }
        public string kid { get; set; }
    }

    public class Cache
    {
        public Cache(string exp)
        {
            this.exp = exp;
        }
        public string exp { get; set; }
    }
}
