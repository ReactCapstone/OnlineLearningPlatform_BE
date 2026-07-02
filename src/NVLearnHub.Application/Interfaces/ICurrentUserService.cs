using System;
using System.Collections.Generic;
using System.Text;

namespace NVLearnHub.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserName { get; }
        int? UserId { get; }
    }
}
