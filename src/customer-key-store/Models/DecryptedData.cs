﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.InformationProtection.Web.Models
{
    //This class implements the format of data returned from the /decrypt API
    //Changes in the returned format will break consuming clients
    public class DecryptedData
    {
        public DecryptedData(string value)
        {
            this.value = value;
        }

        public string value { get; set; }
    }
}