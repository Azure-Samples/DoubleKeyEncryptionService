using System.Security.Claims;
using System.DirectoryServices;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Microsoft.InformationProtection.Web.Models
{
    public class RoleAuthorizer : IAuthorizer
    {
        const string SidClaim = "onprem_sid";
        const string RoleProperty = "memberof";

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

        public void CanUserAccessKey(string sid, KeyStoreData key)
        {
            bool success = false;
            
            DirectoryEntry entry = new DirectoryEntry("LDAP://" + ldapPath);
            DirectorySearcher Dsearch = new DirectorySearcher(entry);

            Dsearch.Filter = "(objectSid=" + sid + ")";

            var result = Dsearch.FindOne();

            if(result == null)
            {
                throw new System.ArgumentException("User not found");
            }

            var memberof = result.Properties[RoleProperty];
            foreach(var member in memberof)
            {
                //Split out the first part of the role to the comma
                var role = GetRole(member.ToString());
                if(!string.IsNullOrEmpty(role) && roles.Contains(role))
                {
                    success = true;
                    break;
                }
            }

            if(!success)
            {
                throw new CustomerKeyStore.Models.KeyAccessException("User does not have access to the key");
            }
        }
        public void CanUserAccessKey(ClaimsPrincipal user, KeyStoreData key)
        {
            string sid = null;

            foreach(var claim in user.Claims)
            {
                if(claim.Type == SidClaim)
                {
                    sid = claim.Value;
                    break;
                }
            }

            if(sid == null)
            {
                throw new System.ArgumentException(SidClaim + " claim not found");
            }

            CanUserAccessKey(sid, key);
        }

        private string ParseCN(string cn)
        {
            //The CN is terminated by a comma
            //A comma can be part of the CN if it is escaped by \ in which case continue searching, adding the comma without the \
            int commaIndex = 3; //Skip over CN=
            string role = string.Empty;

            do
            {
                var newCommaIndex = cn.IndexOf(",", commaIndex);

                if(newCommaIndex != -1)
                {
                    if(cn[newCommaIndex - 1] == '\\')
                    {
                        //Found a delimited comma, skip over, add it to the resulting string, and continue searching
                        role += cn.Substring(commaIndex, newCommaIndex - commaIndex - 1) + ",";
                        newCommaIndex++;
                    }
                    else
                    {
                        role += cn.Substring(commaIndex, newCommaIndex - commaIndex);
                        break;
                    }
                }
                commaIndex = newCommaIndex;
            }
            while(commaIndex > 0 && commaIndex < cn.Length);

            return role;
        }
        
        private string GetRole(string memberOf)
        {
            //Locate CN=<role>,
            int commaIndex = memberOf.IndexOf("CN=");
            return commaIndex == -1 ? string.Empty : ParseCN(memberOf.Substring(commaIndex));
        }        
    }
}