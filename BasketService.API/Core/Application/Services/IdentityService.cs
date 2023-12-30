using System.Security.Claims;

namespace BasketService.API.Core.Application.Services
{
    public class IdentityService : IIdentityService
    {
         IHttpContextAccessor httpContextAccessor;

        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }


        public string GetUserName()
        {
            try
            {
                return httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

            }
            catch
            {
                return "Null User";
            }
        }
    }
}
