using Restorator.Domain.Models.Account;

namespace Restorator.Domain.Services
{
    public delegate void UserLoggedInHandler();
    public delegate void UserLoggedOutHandler();
    public interface ISessionManager
    {
        bool TryGetSession(out SessionInfo sessionInfo);
        bool TryGetToken(out string token);
        void SetSession(SessionInfo sessionInfo, string token);
        bool HaveSession();
        void RemoveSession();

        public static event UserLoggedInHandler? UserLoggedIn;
        public static event UserLoggedOutHandler? UserLoggedOut;
    }
}