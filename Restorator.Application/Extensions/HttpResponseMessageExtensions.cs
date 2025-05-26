using FluentResults;
using System.Net.Http.Json;

namespace Restorator.Application.Client.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<Result> AsResult(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Result.Fail(error);
            }

            return Result.Ok();
        }

        public static async Task<Result<T>> AsResult<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return Result.Fail(error);
            }

            var value = await response.Content.ReadFromJsonAsync<T>();

            if (value is null)
                return Result.Fail("Не удалось десериализовать ответ сервера");

            return Result.Ok(value);
        }
    }
}