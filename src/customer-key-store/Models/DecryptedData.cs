using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.InformationProtection.Web.Models
{
    public class DecryptedData
    {
        public DecryptedData(string value)
        {
            this.value = value;
        }

        public string value { get; set; }
    }
}