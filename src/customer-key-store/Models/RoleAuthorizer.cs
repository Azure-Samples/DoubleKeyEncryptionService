using System.Security.Claims;
using System.DirectoryServices;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Microsoft.InformationProtection.Web.Models
{
    public class RoleAuthorizer : IAuthorizer
    {
        const string sidClaim = "onprem_sid";
        const string roleProperty = "memberof";

        private string ldapPath;
        private HashSet<string> roles = new HashSet<string>();

        public RoleAuthorizer(IConfiguration configuration)
        {
            ldapPath = configuration["RoleAuthorizer:LDAPPath"];
        }

        public void AddRole(string role)
        {
            roles.Add(role);
        }

        private string GetRole(string memberOf)
        {
            string role = string.Empty;
            var splitStrings = memberOf.Split(",");

            //This function obtains the first string in a comma separated string of strings
            //A comma can be delimited by a \ and in that case it should continue searching

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
            
            DirectoryEntry entry = new DirectoryEntry("LDAP://" + ldapPath);
            DirectorySearcher Dsearch = new DirectorySearcher(entry);

            foreach(var claim in user.Claims)
            {
                if(claim.Type == sidClaim)
                {
                    claimFound = true;
                    Dsearch.Filter = "(objectSid=" + claim.Value + ")";
                    break;
                }
            }

            if(!claimFound)
            {
                throw new System.ArgumentException(sidClaim + " claim not found");
            }            

            var result = Dsearch.FindOne();

            if(result == null)
            {
                throw new System.ArgumentException("User not found");
            }

            var memberof = result.Properties[roleProperty];
            foreach(var member in memberof)
            {
                //Split out the first part of the role to the comma
                var role = GetRole(member.ToString());
                if(roles.Contains(role))
                {
                    success = true;
                    break;
                }
            }

            if(!success)
            {
                throw new System.ArgumentException("User does not have access to the key");
            }
        }
    }
}