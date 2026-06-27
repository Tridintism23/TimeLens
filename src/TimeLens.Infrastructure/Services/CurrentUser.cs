using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TimeLens.Application.Common.Interfaces;
using System.Security.Claims;

namespace TimeLens.Infrastructure.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid Id
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?
                    .User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? _httpContextAccessor.HttpContext?
                    .User.FindFirst("sub");

                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        public string Email
            => _httpContextAccessor.HttpContext?
            .User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        public bool IsAuthenticated
            => _httpContextAccessor.HttpContext?
            .User.Identity?.IsAuthenticated ?? false;
    }
}
