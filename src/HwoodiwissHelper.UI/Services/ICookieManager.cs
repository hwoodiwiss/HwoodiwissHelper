namespace HwoodiwissHelper.UI.Services;

public interface ICookieManager
{
    /// <summary>
    /// Gets a cookie value from the browser by name.
    /// </summary>
    /// <param name="cookieName">The name of the cookie to search for.</param>
    /// <returns>A <see cref="Task" /> that resolves to the string value of the cookie indicated by <paramref name="cookieName"/>, or <c>null</c> if the cookie does not exist.</returns>
    Task<string?> GetCookie(string cookieName);
    
    /// <summary>
    /// Deletes a cookie from the browser by name.
    /// </summary>
    /// <param name="cookieName">The name of the cookie to delete.</param>
    /// <returns>A <see cref="Task" /> that resolves when the cookie indicated by <paramref name="cookieName"/> has been deleted.</returns>
    Task DeleteCookie(string cookieName);
}
