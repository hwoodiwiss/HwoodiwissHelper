namespace HwoodiwissHelper.Infrastructure;

public struct TokenWithExpiration<T>(TimeProvider timeProvider, Func<T, DateTime> expirationFactory)
    where T : class?
{
    private DateTime _expiresAt = DateTime.MinValue;
    private T? _token = null;

    public async ValueTask<T> GetOrRenewAsync(Func<ValueTask<T>> tokenFactory)
    {
        if (_token is null || timeProvider.GetUtcNow() >= _expiresAt)
        {
            _token = await tokenFactory();
            _expiresAt = expirationFactory(_token);
        }

        return _token;
    }
}
