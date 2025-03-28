using System.Diagnostics;

namespace HwoodiwissHelper.Core.Extensions;

public static class ResultExtensions
{
    public static async Task<Result<TNextResult, TError>> ThenAsync<TResult, TNextResult, TError>(
        this Task<Result<TResult, TError>> result,
        Func<TResult, Task<Result<TNextResult, TError>>> next)
    {
        var resultValue = await result;
        return resultValue switch
        {
            Result<TResult, TError>.Success success => await next(success.Value),
            Result<TResult, TError>.Failure failure => new Result<TNextResult, TError>.Failure(failure.Error),
            _ => throw new UnreachableException($"Unexpected result variant {resultValue.GetType().Name}"),
        };
    }
    
    public static async Task<Result<TNextResult, TError>> ThenAsync<TResult, TNextResult, TError>(
        this Task<Result<TResult, TError>> result,
        Func<TResult, Result<TNextResult, TError>> next)
    {
        var resultValue = await result;
        return resultValue switch
        {
            Result<TResult, TError>.Success success => next(success.Value),
            Result<TResult, TError>.Failure failure => new Result<TNextResult, TError>.Failure(failure.Error),
            _ => throw new UnreachableException($"Unexpected result variant {resultValue.GetType().Name}"),
        };
    }
    
    public static Result<TResult, TDestError> MapError<TResult, TSourceError, TDestError>(
        this Result<TResult, TSourceError> result,
        Func<TSourceError, TDestError> mapper) =>
        result switch
        {
            Result<TResult, TSourceError>.Success success => new Result<TResult, TDestError>.Success(success.Value),
            Result<TResult, TSourceError>.Failure failure => mapper(failure.Error),
            _ => throw new UnreachableException($"Unexpected result variant {result.GetType().Name}"),
        };
}
