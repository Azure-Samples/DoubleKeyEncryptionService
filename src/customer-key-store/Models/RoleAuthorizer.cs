using System.Security.Claims;
using System.DirectoryServices;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Microsoft.InformationProtection.Web.Models
{
    public class RoleAuthorizer : Authorizer
    {
        const string kSIDClaim = "onprem_sid";
        const string kRoleProperty = "memberof";

        private IConfiguration mConfiguration;
        private HashSet<string> mRoles = new HashSet<string>();

        public RoleAuthorizer(IConfiguration configuration)
        {
            mConfiguration = configuration;
        }

        public void AddRole(string role)
        {
            mRoles.Add(role);
        }

        private string GetRole(string memberOf)
        {
            string role = string.Empty;
            var splitStrings = memberOf.Split(",");

            for(int index = 0; index < splitStrings.Length; index++)
            {
                role += splitStrings[index];
                if(role.Length == 0 || role[role.Length - 1] != '\\')
                {
                    break;
                }
                else
                {
                    //Delimited comma is present, remove the delimiter (\) and add the comma back. Continue searching
                    role = role.Substring(0, role.Length - 1) + ",";
                }
            }

            return role;
        }

        public void CanUserAccessKey(ClaimsPrincipal user, KeyStoreData key)
        {
            bool success = false;
            bool claimFound = false;
            
            DirectoryEntry entry = new DirectoryEntry("LDAP://" + mConfiguration["RoleAuthorizer:LDAPPath"]);
            DirectorySearcher Dsearch = new DirectorySearcher(entry);

            foreach(var claim in user.Claims)
            {
                if(claim.Type == kSIDClaim)
                {
                    claimFound = true;
                    Dsearch.Filter = "(objectSid=" + claim.Value + ")";
                    break;
                }
            }

            if(!claimFound)
            {
                throw new System.Exception(kSIDClaim + " claim not found");
            }            

            var result = Dsearch.FindOne();

            if(result == null)
            {
                throw new System.Exception("User not found");
            }

            var memberof = result.Properties[kRoleProperty];
            foreach(var member in memberof)
            {
                //Split out the first part of the role to the comma
                var role = GetRole(member.ToString());
                if(mRoles.Contains(role))
                {
                    success = true;
                    break;
                }
            }

            if(!success)
            {
                throw new System.Exception("User does not have access to the key");
            }
        }
    }
}