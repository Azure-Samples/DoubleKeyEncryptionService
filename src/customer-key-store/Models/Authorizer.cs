using System.Security.Claims;

namespace Microsoft.InformationProtection.Web.Models
{
    public interface Authorizer
    {
        void CanUserAccessKey(ClaimsPrincipal user, KeyStoreData key);
    }
}