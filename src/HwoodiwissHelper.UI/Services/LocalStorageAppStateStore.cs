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
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            return default;
        }
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
    }

    public async Task RemoveItemAsync(string key)
        => await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
}
