using System;
using AMEBI.Domain.Models;

namespace AMEBI.Domain.Services
{
    public interface IJwtService
    {
        JwtToken CreateToken(Guid userId, string role);
    }
}