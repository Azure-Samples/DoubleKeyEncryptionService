// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    using System;
    using System.Globalization;
    using Microsoft.Extensions.Primitives;
    using Microsoft.InformationProtection.Web.Models.Extensions;

    public static class ProtocolVersionValidator
    {
        private const double MinSupportedVersion = 1.0;
        private const double MaxSupportedVersion = 1.0;
        private const string ProtocolVersion = "protocol-version";

        public static void ValidateProtocolVersion(AspNetCore.Http.HttpRequest request)
        {
            request.ThrowIfNull(nameof(request));

            StringValues values = new StringValues("1.0");  //Older versions of mip were not sending up the protocol, default to 1.0
            if(request.Query.ContainsKey(ProtocolVersion))
            {
                values = request.Query[ProtocolVersion];
            }

            if(values.Count != 1)
            {
                throw new ArgumentException("More than one protocol-version header found");
            }

            if(!double.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double protocolVersion))
            {
                throw new ArgumentException("Unable to parse protocol_version: " + values[0]);
            }

            if(protocolVersion < MinSupportedVersion || protocolVersion > MaxSupportedVersion)
            {
                throw new ArgumentException("Unsupported protocol_version: " + values[0]);
            }
        }
    }
}