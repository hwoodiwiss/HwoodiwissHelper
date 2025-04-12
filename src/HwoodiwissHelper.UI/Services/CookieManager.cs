using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace HwoodiwissHelper.UI.Services;

public sealed class CookieManager : IAsyncDisposable, ICookieManager
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _cookieManager;

    public CookieManager(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <inheritdoc/>
    public async Task<string?> GetCookie(string cookieName)
    {
        await EnsureCookieManager();
        return await _cookieManager!.InvokeAsync<string?>("getCookie", cookieName);
    }

    /// <inheritdoc/>
    public async Task DeleteCookie(string cookieName)
    {
        await EnsureCookieManager(); 
        await _cookieManager!.InvokeVoidAsync("deleteCookie", cookieName);
    }

    public ValueTask DisposeAsync()
    {
        if (_cookieManager is not null)
        {
            return _cookieManager.DisposeAsync();
        }

        return new ValueTask();
    }
    
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
