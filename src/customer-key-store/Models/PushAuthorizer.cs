// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.InformationProtection.Web.Models.Extensions;

    public class PushAuthorizer : IAuthorizer
    {
        public PushAuthorizer(string pushService)
        {
            PushService = pushService;
        }

        public string PushService { get; private set; }

        public Task ProcessAccessRequest(ClaimsPrincipal user, KeyStoreData key)
        {
            user.ThrowIfNull(nameof(user));
            var email = EmailAuthorizer.GetEmailFromClaims(user);

            //send email to push notification service
            return Task.FromResult(true);
        }
    }
}