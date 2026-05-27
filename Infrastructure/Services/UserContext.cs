using System.Security.Claims;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _accessor;

        public UserContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public int GetUserId()
        {
       
            var idClaim = _accessor.HttpContext?.User?.FindFirst("EmpresaId")?.Value
                          ?? _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(idClaim, out var id) ? id : 0;
        }

        public string GetRole()
        {
            return _accessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "Guest";
        }

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}