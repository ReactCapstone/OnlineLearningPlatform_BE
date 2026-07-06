using Microsoft.AspNetCore.Http;
using NVLearnHub.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService  
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        public int? UserId =>
            int.TryParse(
                _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value,
                out var id) ? id : null;
    }
}
