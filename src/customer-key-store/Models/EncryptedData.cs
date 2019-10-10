using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.InformationProtection.Web.Models
{
    public class EncryptedData
    {
        public string alg { get; set; }
        public string value { get; set; }
    }
}