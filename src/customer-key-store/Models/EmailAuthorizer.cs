using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Microsoft.InformationProtection.Web.Models
{
    public class EmailAuthorizer : Authorizer
    {
        const string kEmailClaim = ClaimTypes.Email;
        const string kUpnClaim = ClaimTypes.Upn;
        private HashSet<string> mValidEmails = new HashSet<string>();

        public void AddEmail(string email)
        {
            mValidEmails.Add(email);
        }

        public void CanUserAccessKey(ClaimsPrincipal user, KeyStoreData key)
        {
            string email = null;

            foreach(var claim in user.Claims)
            {
                if(claim.Type == kEmailClaim)
                {
                    email = claim.Value;
                    break;
                }
                else if(claim.Type == kUpnClaim)
                {
                    email = claim.Value;
                    break;
                }                
            }

            if(email == null)
            {
                throw new System.Exception("The email or upn claim is required");
            }

            if(!mValidEmails.Contains(email.Trim().ToLower()))
            {
                throw new System.Exception("User does not have access to the key");
            }
        }
    }
}