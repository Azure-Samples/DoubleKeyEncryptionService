// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    //This class implements the format of data returned from the /decrypt API
    //Changes in the returned format will break consuming clients
    public class DecryptedData
    {
        public DecryptedData(string value)
        {
            this.Value = value;
        }

        [Newtonsoft.Json.JsonProperty("value")]
        public string Value { get; private set; }
    }
}