using FluentResults;
using Restorator.Application.Client.Extensions;
using Restorator.Application.Client.Services.Abstract;
using Restorator.Domain.Models.Account;
using Restorator.Domain.Services;
using System.Net.Http.Json;

namespace Restorator.Application.Client.Services
{
    public class AccountService : ApiClientBase, IAccountService
    {
        private readonly ISessionManager _sessionManager;
        public AccountService(HttpClient client, ISessionManager sessionManager) : base(client, "account")
        {
            _sessionManager = sessionManager;
        }


        public async Task<Result<SessionInfo>> GetSessionInfoAsync()//Why?
        {
            var sessionInfo = await GetFromJsonAsync<SessionInfo>("/info");

            return sessionInfo.ToResultWithNullCheck();
        }

        public async Task<Result> RequestPasswordReset(string email)
        {
            var response = await PostAsJsonAsync("/reset", email);

            return await response.AsResult();
        }

        public async Task<Result<AuthorizationResult>> SignInAsync(SignInDTO model)
        {
            var response = await PostAsJsonAsync("/signin", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Result.Fail(error);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthorizationResult>();

            if (result is null)
                return Result.Fail("");

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

            _sessionManager.SetSession(result.SessionInfo, result.Token);

            return result;
        }

        public async Task<Result<AuthorizationResult>> SignInAsync(RecoverAccountDTO model)
        {
            var response = await PostAsJsonAsync("/recover", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Result.Fail(error);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthorizationResult>();

            if (result is null)
                return Result.Fail("");

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

            _sessionManager.SetSession(result.SessionInfo, result.Token);

            return result;
        }

        public async Task<Result> SignUpAsync(SignUpDTO model)
        {
            var response = await PostAsJsonAsync("/signup", model);

            return await response.AsResult();
        }

        public async Task<Result> UpdatePassword(string password)
        {
            var response = await PatchAsJsonAsync(string.Empty, password);

            return await response.AsResult();
        }
    }
}