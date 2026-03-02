namespace HwoodiwissHelper.UI.Services;

internal interface IAppStateStore
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
}
