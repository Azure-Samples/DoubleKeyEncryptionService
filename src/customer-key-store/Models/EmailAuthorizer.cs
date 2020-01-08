// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    using System.Collections.Generic;
    using System.Security.Claims;

    using Microsoft.InformationProtection.Web.Models.Extensions;
    public class EmailAuthorizer : IAuthorizer
    {
        private const string EmailClaim = ClaimTypes.Email;
        private const string UpnClaim = ClaimTypes.Upn;
        private HashSet<string> validEmails = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

        public void AddEmail(string email)
        {
            email.ThrowIfNull(nameof(email));

            validEmails.Add(email.Trim());
        }

        public void CanUserAccessKey(ClaimsPrincipal user, KeyStoreData key)
        {
            string email = null;

            user.ThrowIfNull(nameof(user));

            foreach(var claim in user.Claims)
            {
                if(claim.Type == EmailClaim)
                {
                    email = claim.Value;
                    break;
                }
                else if(claim.Type == UpnClaim)
                {
                    email = claim.Value;
                    break;
                }
            }

            if(email == null)
            {
                throw new System.ArgumentException("The email or upn claim is required");
            }

            if(!validEmails.Contains(email.Trim()))
            {
                throw new CustomerKeyStore.Models.KeyAccessException("User does not have access to the key");
            }
        }
    }
}