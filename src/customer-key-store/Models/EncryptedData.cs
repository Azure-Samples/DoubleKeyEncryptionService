﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.InformationProtection.Web.Models
{
    //This class implements the format of data accepted in the /decrypt API
    //Changes in the format will break consuming clients  
    public class EncryptedData
    {
        public string alg { get; set; }
        public string value { get; set; }
    }
}