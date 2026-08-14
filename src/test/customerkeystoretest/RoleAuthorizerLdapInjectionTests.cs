// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Customerkeystoretest
{
    using Microsoft.InformationProtection.Web.Models;
    using Xunit;

    public class RoleAuthorizerLdapInjectionTests
    {
        [Fact]
        public void EscapeLdapSearchFilter_LeavesPlainSidUnchanged()
        {
            const string sid = "S-1-5-21-1004336348-1177238915-682003330-512";
            Assert.Equal(sid, RoleAuthorizer.EscapeLdapSearchFilter(sid));
        }

        [Theory]
        [InlineData("\\", "\\5c")]
        [InlineData("*", "\\2a")]
        [InlineData("(", "\\28")]
        [InlineData(")", "\\29")]
        [InlineData("\0", "\\00")]
        [InlineData("/", "\\2f")]
        public void EscapeLdapSearchFilter_EscapesReservedCharacters(string input, string expected)
        {
            Assert.Equal(expected, RoleAuthorizer.EscapeLdapSearchFilter(input));
        }

        [Theory]
        // Classic break-out: close the objectSid clause and inject an OR for a privileged group.
        [InlineData("*)(memberof=CN=Admins", "\\2a\\29\\28memberof=CN=Admins")]
        // Wildcard-only value that would otherwise match every object.
        [InlineData("*", "\\2a")]
        // Append an always-true objectClass filter.
        [InlineData("foo)(objectClass=*", "foo\\29\\28objectClass=\\2a")]
        // Nested filter with OR operator and multiple wildcards.
        [InlineData("*)(uid=*))(|(uid=*", "\\2a\\29\\28uid=\\2a\\29\\29\\28|\\28uid=\\2a")]
        // Empty-AND tautology injection.
        [InlineData("admin)(&)", "admin\\29\\28&\\29")]
        // Attempt to smuggle a raw backslash to defeat escaping.
        [InlineData("x\\)(cn=*", "x\\5c\\29\\28cn=\\2a")]
        // NUL-byte truncation attempt.
        [InlineData("S-1-5-21\0)(objectClass=*", "S-1-5-21\\00\\29\\28objectClass=\\2a")]
        public void EscapeLdapSearchFilter_NeutralizesInjectionPayload(string payload, string expected)
        {
            string escaped = RoleAuthorizer.EscapeLdapSearchFilter(payload);

            // The characters that give the payload its filter syntax must all be neutralized,
            // so the value can no longer alter the structure of the LDAP filter.
            Assert.DoesNotContain("*", escaped);
            Assert.DoesNotContain("(", escaped);
            Assert.DoesNotContain(")", escaped);
            Assert.Equal(expected, escaped);
        }
    }
}
