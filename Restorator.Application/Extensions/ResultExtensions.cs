using FluentResults;

namespace Restorator.Application.Client.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> ToResultWithNullCheck<T>(this T? obj)
        {
            if (obj is null)
                return Result.Fail("");

            return obj;
        }
    }
}