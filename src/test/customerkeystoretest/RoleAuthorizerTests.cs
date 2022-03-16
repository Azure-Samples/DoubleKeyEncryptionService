// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Customerkeystoretest
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using Microsoft.InformationProtection.Web.Models;
    using Xunit;

    public class RoleAuthorizerTests
    {
        [Fact]
        public void NoValidLDAPPath_Fail()
        {
            Assert.Throws<System.ArgumentException>(() => new RoleAuthorizer(new ConfigurationMock()));
        }

       [Fact]
        public void ValidLDAPPath_Success()
        {
            ConfigurationMock configurationMock = new ConfigurationMock();
            configurationMock["RoleAuthorizer:LDAPPath"] = "test";
            var roleAuthorizer = new RoleAuthorizerMock(configurationMock);
        }

        [Fact]
        public void NoClaims_Fail()
        {
            ConfigurationMock configurationMock = new ConfigurationMock();
            configurationMock["RoleAuthorizer:LDAPPath"] = "test";
            var roleAuthorizer = new RoleAuthorizerMock(configurationMock);
            ClaimsPrincipal principal = new ClaimsPrincipal();
            Assert.Throws<System.ArgumentException>(() => roleAuthorizer.CanUserAccessKey(principal, null));
        }

       [Fact]
        public void NoValidRoles_Fail()
        {
            ConfigurationMock configurationMock = new ConfigurationMock();
            configurationMock["RoleAuthorizer:LDAPPath"] = "test";
            var roleAuthorizer = new RoleAuthorizerMock(configurationMock);
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim("onprem_sid", "345435")});
            ClaimsPrincipal principal = new ClaimsPrincipal();
            principal.AddIdentity(identity);
            Assert.Throws<CustomerKeyStore.Models.KeyAccessException>(() => roleAuthorizer.CanUserAccessKey(principal, null));
        }

       [Fact]
        public void NotMemberOfRoles_Fail()
        {
            ConfigurationMock configurationMock = new ConfigurationMock();
            configurationMock["RoleAuthorizer:LDAPPath"] = "test";
            var roleAuthorizer = new RoleAuthorizerMock(configurationMock);
            roleAuthorizer.AddRole("testgroup");  //testgroup is allowed access to the key
            roleAuthorizer.AddMemberOf("345435", "CN=othergroup");   //sid is a member of testgroup

            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim("onprem_sid", "345435")});
            ClaimsPrincipal principal = new ClaimsPrincipal();
            principal.AddIdentity(identity);
            Assert.Throws<CustomerKeyStore.Models.KeyAccessException>(() => roleAuthorizer.CanUserAccessKey(principal, null));
        }

       [Fact]
        public void ValidRoles_OneSid_Success()
        {
            ConfigurationMock configurationMock = new ConfigurationMock();
            configurationMock["RoleAuthorizer:LDAPPath"] = "test";
            var roleAuthorizer = new RoleAuthorizerMock(configurationMock);
            roleAuthorizer.AddRole("testgroup");  //testgroup is allowed access to the key
            roleAuthorizer.AddMemberOf("345435", "CN=testgroup3");   //sid is a member of testgroup3
            roleAuthorizer.AddMemberOf("345435", "CN=testgroup");   //sid is a member of testgroup

            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim("onprem_sid", "345435")}); //User's sid is 345435
            ClaimsPrincipal principal = new ClaimsPrincipal();
            principal.AddIdentity(identity);

            roleAuthorizer.CanUserAccessKey(principal, null);
        }

        [Fact]
        public void ValidRoles_ManySid_Success()
        {
            ConfigurationMock configurationMock = new ConfigurationMock();
            configurationMock["RoleAuthorizer:LDAPPath"] = "test";
            var roleAuthorizer = new RoleAuthorizerMock(configurationMock);

            //Below are valid roles
            roleAuthorizer.AddRole("testgr,oup1");
            roleAuthorizer.AddRole("testgroup2");
            roleAuthorizer.AddRole("testgroup5");

            //Each sid is a member of the following groups
            roleAuthorizer.AddMemberOf("0987635457", "CN=testgr\\,oup3,");
            roleAuthorizer.AddMemberOf("0987635457", "CN=testgr\\,oup1,");
            roleAuthorizer.AddMemberOf("2547546374", "CN=testgroup2,");
            roleAuthorizer.AddMemberOf("86575765567", "CN=testgroup3,");
            roleAuthorizer.AddMemberOf("86575765567", "CN=testgroup1,");
            roleAuthorizer.AddMemberOf("86575765567", "CN=testgroup3,");
            roleAuthorizer.AddMemberOf("86575765567", "CN=testgroup5,");
            roleAuthorizer.AddMemberOf("4576588653", "CN=othergroup,");

            ClaimsPrincipal principalUser1 = new ClaimsPrincipal();
            ClaimsIdentity identityUser1 = new ClaimsIdentity(new List<Claim>(){new Claim("onprem_sid", "0987635457")});
            principalUser1.AddIdentity(identityUser1);
            roleAuthorizer.CanUserAccessKey(principalUser1, null);

            ClaimsPrincipal principalUser2 = new ClaimsPrincipal();
            ClaimsIdentity identityUser2 = new ClaimsIdentity(new List<Claim>(){new Claim("onprem_sid", "2547546374")});
            principalUser2.AddIdentity(identityUser2);
            roleAuthorizer.CanUserAccessKey(principalUser2, null);

            ClaimsPrincipal principalUser3 = new ClaimsPrincipal();
            ClaimsIdentity identityUser3 = new ClaimsIdentity(new List<Claim>(){new Claim("onprem_sid", "86575765567")});
            principalUser3.AddIdentity(identityUser3);
            roleAuthorizer.CanUserAccessKey(principalUser3, null);

            ClaimsPrincipal principalUser4 = new ClaimsPrincipal();
            ClaimsIdentity identityUser4 = new ClaimsIdentity(new List<Claim>(){new Claim("onprem_sid", "4576588653")});
            principalUser4.AddIdentity(identityUser4);
            Assert.Throws<CustomerKeyStore.Models.KeyAccessException>(() => roleAuthorizer.CanUserAccessKey(principalUser4, null));
        }
    }
}
