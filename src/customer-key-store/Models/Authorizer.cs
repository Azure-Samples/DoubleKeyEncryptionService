// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.InformationProtection.Web.Models
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    public interface IAuthorizer
    {
        Task ProcessAccessRequest(ClaimsPrincipal user, KeyStoreData key);
    }
}