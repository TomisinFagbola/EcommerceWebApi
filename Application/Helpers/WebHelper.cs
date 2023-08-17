using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Helpers
{
    public interface IWebHelper
    {
        Guid GetUserId();
        HttpContext HttpContext();
    }
    public class WebHelper : IWebHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static ClaimsPrincipal CurrentUser
        {
            get
            {
                if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User != null)
                {
                    return _httpContextAccessor.HttpContext.User;
                }

                return _httpContextAccessor.HttpContext.User;
            }
        }

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpContext HttpContext()
        {
            return _httpContextAccessor.HttpContext;
        }

        public static Guid UserId
        {
            get
            {
                Guid id;

                var userId = _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == ClaimTypeHelper.UserId).FirstOrDefault()?.Value ?? "";

                Guid.TryParse(userId, out id);

                return id;
            }
        }

        public static IEnumerable<string> Roles =>
            _httpContextAccessor.HttpContext != null ?
                _httpContextAccessor.HttpContext.User.UserClaims().Roles.ToList() : new List<string>();


        public static string FirstName =>
            _httpContextAccessor?.HttpContext?.User?.Claims?.Where(x => x.Type == ClaimTypeHelper.FirstName).FirstOrDefault()?.Value ?? "";
        public static string LastName =>
           _httpContextAccessor?.HttpContext?.User?.Claims?.Where(x => x.Type == ClaimTypeHelper.LastName).FirstOrDefault()?.Value ?? "";
        public static string Email =>
           _httpContextAccessor?.HttpContext?.User?.Claims?.Where(x => x.Type == ClaimTypeHelper.Email).FirstOrDefault()?.Value ?? "";
        public static string PhoneNumber =>
         _httpContextAccessor?.HttpContext?.User?.Claims?.Where(x => x.Type == ClaimTypeHelper.PhoneNumber).FirstOrDefault()?.Value ?? "";

        public Guid GetUserId()
        {
            return UserId;
        }
    }
}
