// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.InformationProtection.Web.Models
{
    using System.Security.Claims;
    public interface IAuthorizer
    {
        void CanUserAccessKey(ClaimsPrincipal user, KeyStoreData key);
    }
}