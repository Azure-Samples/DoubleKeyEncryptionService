// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    //This class implements the format of data accepted in the /decrypt API
    //Changes in the format will break consuming clients
    public class EncryptedData
    {
        [Newtonsoft.Json.JsonProperty("alg")]
        public string Algorithm { get; set; }

        [Newtonsoft.Json.JsonProperty("value")]
        public string Value { get; set; }
    }
}