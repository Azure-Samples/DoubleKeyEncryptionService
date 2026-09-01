// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.InformationProtection.Web.Models.Extensions;

    public class MultiFactorAuthorizer : IAuthorizer
    {
        private IAuthorizer mPrimaryAuthorizer;
        private IAuthorizer mSecondaryAuthorizer;

        public MultiFactorAuthorizer(IAuthorizer primaryAuthorizer, IAuthorizer secondaryAuthorizer)
        {
            mPrimaryAuthorizer = primaryAuthorizer;
            mSecondaryAuthorizer = secondaryAuthorizer;
        }

        public async Task ProcessAccessRequest(ClaimsPrincipal user, KeyStoreData key)
        {
            user.ThrowIfNull(nameof(user));

            await mPrimaryAuthorizer.ProcessAccessRequest(user, key).ConfigureAwait(false);

            await mSecondaryAuthorizer.ProcessAccessRequest(user, key).ConfigureAwait(false);
        }
    }
}