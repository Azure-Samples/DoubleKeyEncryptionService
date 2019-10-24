using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Microsoft.InformationProtection.Web.Models
{
    public class EmailAuthorizer : IAuthorizer
    {
        const string emailClaim = ClaimTypes.Email;
        const string upnClaim = ClaimTypes.Upn;
        private HashSet<string> validEmails = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

        public void AddEmail(string email)
        {
            validEmails.Add(email.Trim());
        }

        public void CanUserAccessKey(ClaimsPrincipal user, KeyStoreData key)
        {
            string email = null;

            foreach(var claim in user.Claims)
            {
                if(claim.Type == emailClaim)
                {
                    email = claim.Value;
                    break;
                }
                else if(claim.Type == upnClaim)
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
                throw new System.ArgumentException("User does not have access to the key");
            }
        }
    }
}