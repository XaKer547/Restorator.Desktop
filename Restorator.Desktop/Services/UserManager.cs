using Restorator.Domain.Services;

namespace Restorator.Desktop.Services
{
    public class UserManager : IUserManager
    {
        public bool TryGetUserId(out int userId)
        {
            userId = default;

            return true;
        }
    }
}
