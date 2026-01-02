using CarePoint.API.Models; 

namespace CarePoint.API.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}

