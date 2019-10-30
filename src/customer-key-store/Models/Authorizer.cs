using System.Security.Claims;

namespace Microsoft.InformationProtection.Web.Models
{
    public interface IAuthorizer
    {
        void CanUserAccessKey(ClaimsPrincipal user, KeyStoreData key);
    }
}