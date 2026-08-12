using System.Text.Json;
using Microsoft.JSInterop;

namespace HwoodiwissHelper.UI.Services;

internal sealed class LocalStorageAppStateStore(IJSRuntime jsRuntime) : IAppStateStore
{
    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
            if (json is null)
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            // Corrupted or unreadable — remove it so the caller gets a clean slate
            try
            {
                await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            }
            catch
            {
                // Ignore cleanup failures; caller still gets default value
            }

            return default;
        }
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch
        {
            // Storage write failed; continue without persisting state
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch
        {
            // Storage remove failed; continue without throwing
        }
    }
}
