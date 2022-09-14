// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Customerkeystoretest
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using Xunit;
    
    using Microsoft.InformationProtection.Web.Models;
    public class EmailAuthorizerTests
    {
        [Fact]
        public void NoClaims_Fail()
        {
            EmailAuthorizer auth = new EmailAuthorizer();
            ClaimsPrincipal principal = new ClaimsPrincipal();
            Assert.Throws<System.ArgumentException>(() => auth.CanUserAccessKey(principal, null));
        }

        [Theory]
        [InlineData(ClaimTypes.Email)]
        [InlineData(ClaimTypes.Upn)]
        public void NoValidEmails_Fail(string claimType)
        {
            EmailAuthorizer auth = new EmailAuthorizer();
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim(claimType, "testuser@contoso.com")});
            principal.AddIdentity(identity);
            Assert.Throws<CustomerKeyStore.Models.KeyAccessException>(() => auth.CanUserAccessKey(principal, null));
        }

        [Theory]
        [InlineData(ClaimTypes.Email)]
        [InlineData(ClaimTypes.Upn)]
        public void ValidEmail_OneEmail_Success(string claimType)
        {
            EmailAuthorizer auth = new EmailAuthorizer();
            auth.AddEmail("testuser@contoso.com");
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim(claimType, "testuser@contoso.com")});
            principal.AddIdentity(identity);
            auth.CanUserAccessKey(principal, null);
        }

        [Theory]
        [InlineData(ClaimTypes.Email)]
        [InlineData(ClaimTypes.Upn)]
        public void ValidEmail_ManyEmails_Success(string claimType)
        {
            EmailAuthorizer auth = new EmailAuthorizer();
            auth.AddEmail("testuser@contoso.com");
            auth.AddEmail("testuser4@contoso.com");
            auth.AddEmail("testuser2@contoso.com");
            ClaimsPrincipal principalUser1 = new ClaimsPrincipal();
            ClaimsIdentity identityUser1 = new ClaimsIdentity(new List<Claim>(){new Claim(claimType, "testuser@contoso.com")});
            principalUser1.AddIdentity(identityUser1);
            auth.CanUserAccessKey(principalUser1, null);

            ClaimsPrincipal principalUser2 = new ClaimsPrincipal();
            ClaimsIdentity identityUser2 = new ClaimsIdentity(new List<Claim>(){new Claim(claimType, "testuser4@contoso.com")});
            principalUser2.AddIdentity(identityUser2);
            auth.CanUserAccessKey(principalUser2, null);

            ClaimsPrincipal principalUser3 = new ClaimsPrincipal();
            ClaimsIdentity identityUser3 = new ClaimsIdentity(new List<Claim>(){new Claim(claimType, "testuser2@contoso.com")});
            principalUser3.AddIdentity(identityUser3);
            auth.CanUserAccessKey(principalUser3, null);
        }
    }
}
