using Microsoft.JSInterop;

namespace HwoodiwissHelper.UI.Services;

public sealed class CookieManager(IJSRuntime jsRuntime) : IAsyncDisposable, ICookieManager
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private IJSObjectReference? _cookieManager;

    /// <inheritdoc/>
    public async Task<string?> GetCookieAsync(string cookieName)
    {
        await EnsureCookieManager();
        return await _cookieManager!.InvokeAsync<string?>("getCookie", cookieName);
    }

    /// <inheritdoc/>
    public async Task DeleteCookieAsync(string cookieName)
    {
        await EnsureCookieManager();
        await _cookieManager!.InvokeVoidAsync("deleteCookie", cookieName);
    }

    public ValueTask DisposeAsync() =>
        _cookieManager is not null ? _cookieManager.DisposeAsync() : new ValueTask();

    private async Task EnsureCookieManager()
    {
        if (_cookieManager is not null)
        {
            return;
        }

        _cookieManager = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/cookieManager.js");

        if (_cookieManager is null)
        {
            throw new InvalidOperationException("Failed to load cookie manager JS Module.");
        }
    }
}
