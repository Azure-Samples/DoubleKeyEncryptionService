namespace customerkeystoretest
{
    using System;
    using System.Security.Claims;
    using Xunit;
    
    using Microsoft.InformationProtection.Web.Models;
    public class EmailAuthorizerTests
    {
        [Fact]
        public void Test1()
        {
            EmailAuthorizer auth = new EmailAuthorizer();
            ClaimsPrincipal principal = new ClaimsPrincipal();
            auth.CanUserAccessKey();
            Assert.NotEqual(expected, actual);
        }
    }
}
