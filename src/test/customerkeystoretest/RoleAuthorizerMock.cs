// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Customerkeystoretest
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.InformationProtection.Web.Models;

    public class RoleAuthorizerMock : RoleAuthorizer
    {
        private Dictionary<string, List<string>> mMembersOf = new Dictionary<string, List<string>>();

        public RoleAuthorizerMock(IConfiguration configuration) : base(configuration)
        {
        }

        public void AddMemberOf(string sid, string memberOf)
        {
            if(!mMembersOf.ContainsKey(sid))
            {
                mMembersOf[sid] = new List<string>();
            }
            mMembersOf[sid].Add(memberOf);
        }

        protected override IEnumerable<string> GetMembersOfFromSID(string sid)
        {
            return mMembersOf.GetValueOrDefault(sid, new List<string>());
        }
    }
}
