using System.Security.Claims;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace MyMisWeb.Services
{
    public interface ITokenCacheFactory
    {
        TokenCache CreateForUser(ClaimsPrincipal user);
    }
}