using Restorator.Desktop.Properties;
using Restorator.Domain.Models.Account;
using Restorator.Domain.Services;

namespace Restorator.Desktop.Session
{
    public class SessionManager : ISessionManager
    {
        private readonly Settings _settings = Settings.Default;

        public event UserLoggedInHandler? UserLoggedIn;
        public event UserLoggedOutHandler? UserLoggedOut;
        public bool TryGetSession(out SessionInfo sessionInfo)
        {
            if (!HaveSession())
            {
                sessionInfo = null;
                return false;
            }

            sessionInfo = new(_settings.Username, _settings.Role);

            return true;
        }
        public void RemoveSession()
        {
            RemoveSessionRaw();

            UserLoggedOut?.Invoke();
        }
        public void SetSession(SessionInfo sessionInfo, string token)
        {
            RemoveSessionRaw();

            SetSessionRaw(sessionInfo, token);

            UserLoggedIn?.Invoke();
        }
        public void SetSessionWithoutNotify(SessionInfo sessionInfo, string token)
        {
            SetSessionRaw(sessionInfo, token);
        }

        public bool HaveSession() => _settings.Token != string.Empty;
        public bool TryGetToken(out string token)
        {
            if (HaveSession())
            {
                token = _settings.Token;

                return true;
            }

            token = null;

            return false;
        }

        private void RemoveSessionRaw()
        {
            _settings.Reset();

            _settings.Save();
        }
        private void SetSessionRaw(SessionInfo sessionInfo, string token)
        {
            _settings.Token = token;

            _settings.Role = sessionInfo.Role;

            _settings.Username = sessionInfo.Username;

            _settings.Save();
        }
    }
}