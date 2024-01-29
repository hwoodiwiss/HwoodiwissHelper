namespace HwoodiwissHelper.Infrastructure;

public struct TokenWithExpiration<T>(TimeProvider timeProvider, Func<T, DateTime> expirationFactory)
    where T : class?
{
    private DateTime _expiresAt = DateTime.MinValue;
    private T? _token = null;

    public T GetOrRenew(Func<T> tokenFactory)
    {
        if (_token is null || timeProvider.GetUtcNow() >= _expiresAt)
        {
            _token = tokenFactory();
            _expiresAt = expirationFactory(_token);
        }

        return _token;
    }
}
