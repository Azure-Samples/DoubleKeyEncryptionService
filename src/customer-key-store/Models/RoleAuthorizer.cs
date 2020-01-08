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

        public void CanUserAccessKey(ClaimsPrincipal user, KeyStoreData key)
        {
            user.ThrowIfNull(nameof(user));

            bool success = false;
            bool claimFound = false;

            using(DirectoryEntry entry = new DirectoryEntry("LDAP://" + ldapPath))
            {
                using(DirectorySearcher dSearch = new DirectorySearcher(entry))
                {
                    foreach(var claim in user.Claims)
                    {
                        if(claim.Type == SidClaim)
                        {
                            claimFound = true;
                            dSearch.Filter = "(objectSid=" + claim.Value + ")";
                            break;
                        }
                    }

                    if(!claimFound)
                    {
                        throw new System.ArgumentException(SidClaim + " claim not found");
                    }

                    var result = dSearch.FindOne();

                    if(result == null)
                    {
                        throw new System.ArgumentException("User not found");
                    }

                    var memberof = result.Properties[RoleProperty];
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
                        throw new CustomerKeyStore.Models.KeyAccessException("User does not have access to the key");
                    }
                }
            }
        }

        private string GetRole(string memberOf)
        {
            string role = string.Empty;
            var splitStrings = memberOf.Split(",");

            //This function obtains the first string in a comma separated string of strings
            //A comma can be escaped by a \ and in that case it should continue searching

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
    }
}