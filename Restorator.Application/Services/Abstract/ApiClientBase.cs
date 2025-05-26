using System.Net.Http.Json;

namespace Restorator.Application.Client.Services.Abstract
{
    public abstract class ApiClientBase
    {
        public readonly string Endpoint;
        protected readonly HttpClient _client;
        protected ApiClientBase(HttpClient client, string enpoint)
        {
            _client = client;
            Endpoint = enpoint;
        }

        public Task<HttpResponseMessage> PostAsJsonAsync<TValue>(string? uri, TValue value, CancellationToken cancellationToken = default)
        {
            return _client.PostAsJsonAsync($"/api/{Endpoint}{uri}", value, cancellationToken);
        }
        public Task<HttpResponseMessage> PutAsJsonAsync<TValue>(string? uri, TValue value, CancellationToken cancellationToken = default)
        {
            return _client.PutAsJsonAsync($"/api/{Endpoint}{uri}", value, cancellationToken);
        }
        public Task<T?> GetFromJsonAsync<T>(string? uri, CancellationToken cancellationToken = default)
        {
            return _client.GetFromJsonAsync<T>($"/api/{Endpoint}{uri}", cancellationToken);
        }
        public Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(string? uri, TValue value, CancellationToken cancellationToken = default)
        {
            return _client.PatchAsJsonAsync($"/api/{Endpoint}{uri}", value, cancellationToken);
        }
        public Task<HttpResponseMessage> DeleteAsync(string? uri, CancellationToken cancellationToken = default)
        {
            return _client.DeleteAsync($"/api/{Endpoint}{uri}", cancellationToken);
        }
        public Task<HttpResponseMessage> HeadAsync(string? uri, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Head, $"/api/{Endpoint}{uri}");

            return _client.SendAsync(request, cancellationToken);
        }
    }
}
