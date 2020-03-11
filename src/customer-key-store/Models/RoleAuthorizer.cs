// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.Security.Claims;
    using Microsoft.Extensions.Configuration;
    using Microsoft.InformationProtection.Web.Models.Extensions;
    public class RoleAuthorizer : IAuthorizer
    {
        private const string SidClaim = "onprem_sid";
        private const string RoleProperty = "memberof";

        private string ldapPath;
        private HashSet<string> roles = new HashSet<string>();

        public RoleAuthorizer(IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            ldapPath = configuration["RoleAuthorizer:LDAPPath"];
        }

        public void AddRole(string role)
        {
            roles.Add(role);
        }

        public void CanUserAccessKey(string sid)
        {
            sid.ThrowIfNull(nameof(sid));

            using(DirectoryEntry entry = new DirectoryEntry("LDAP://" + ldapPath))
            {
                using(DirectorySearcher dSearch = new DirectorySearcher(entry))
                {
                    dSearch.Filter = "(objectSid=" + sid + ")";

                    var result = dSearch.FindOne();

                    if(result == null)
                    {
                        throw new System.ArgumentException("User not found");
                    }

                    var memberof = result.Properties[RoleProperty];
                    bool success = false;
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
            }
        }

        public void CanUserAccessKey(ClaimsPrincipal user, KeyStoreData key)
        {
            user.ThrowIfNull(nameof(user));

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

            CanUserAccessKey(sid);
        }

        private static string ParseCN(string distinguishedName)
        {
            distinguishedName.ThrowIfNull(nameof(distinguishedName));

            //The CN is terminated by a comma
            //A comma can be part of the CN if it is escaped by \ in which case continue searching, adding the comma without the \
            int commaIndex = distinguishedName.IndexOf("CN=", System.StringComparison.InvariantCulture);

            if(commaIndex == -1)
            {
                return string.Empty;
            }

            System.Text.StringBuilder role = new System.Text.StringBuilder();
            commaIndex += 3; //Skip over CN=
            do
            {
                var newCommaIndex = distinguishedName.IndexOf(",", commaIndex, System.StringComparison.InvariantCulture);

                if(newCommaIndex != -1)
                {
                    if(distinguishedName[newCommaIndex - 1] == '\\')
                    {
                        //Found a delimited comma, skip over, add it to the resulting string, and continue searching
                        role.Append(distinguishedName.Substring(commaIndex, newCommaIndex - commaIndex - 1)).Append(",");
                        newCommaIndex++;
                    }
                    else
                    {
                        role.Append(distinguishedName.Substring(commaIndex, newCommaIndex - commaIndex));
                        break;
                    }
                }
                else
                {
                    role.Append(distinguishedName.Substring(commaIndex));
                    break;
                }

                commaIndex = newCommaIndex;
            }
            while(commaIndex > 0 && commaIndex < distinguishedName.Length);

            return role.ToString();
        }

        public static string GetRole(string memberOf)
        {
            memberOf.ThrowIfNull(nameof(memberOf));
            return ParseCN(memberOf);
        }
    }
}