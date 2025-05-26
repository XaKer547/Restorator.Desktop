namespace Restorator.Domain.Services
{
    public interface IJwtService
    {
        public string CreateToken(int userId, string role);
    }
}
