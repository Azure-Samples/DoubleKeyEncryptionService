// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    //This class implements the format of data accepted in the /decrypt API
    //Changes in the format will break consuming clients
    //See src\customer-key-store\Protocols\Decrypt.Request.json
    public class EncryptedData
    {
        /// <summary>
        /// Gets the algorithm
        /// </summary>
        /// <remarks>
        /// The algorithm used to encrypt the data.  Currently only RSA-OAEP-256 is supported
        /// Required.
        /// Valid values:
        ///  - RSA-OAEP-256 - RSA OAEP encoding with SHA-256
        /// </remarks>
        [Newtonsoft.Json.JsonProperty("alg")]
        public string Algorithm { get; set; }

        /// <summary>
        /// Gets the value
        /// </summary>
        /// <remarks>
        /// The encrypted data in base 64 format
        /// Required.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty("value")]
        public string Value { get; set; }
    }
}