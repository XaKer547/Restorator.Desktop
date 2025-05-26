namespace Restorator.Domain.Models.Account
{
    public class AuthorizationResult
    {
        public AuthorizationResult(SessionInfo sessionInfo, string token)
        {
            SessionInfo = sessionInfo;
            Token = token;
        }

        public SessionInfo SessionInfo { get; set; }
        public string Token { get; set; }
    }
}