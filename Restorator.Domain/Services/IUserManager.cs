namespace Restorator.Domain.Services
{
    public interface IUserManager
    {
        public bool TryGetUserId(out int userId);
    }
}
