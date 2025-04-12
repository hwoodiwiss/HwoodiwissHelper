export function getCookie(cookieName) {
    var name = cookieName + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var cookies = decodedCookie.split(";");
    for (var _i = 0, cookies_1 = cookies; _i < cookies_1.length; _i++) {
        var cookie = cookies_1[_i];
        cookie = cookie.trim();
        if (cookie.indexOf(name) === 0) {
            return cookie.substring(name.length, cookie.length);
        }
    }
    return null;
}
export function deleteCookie(cookieName) {
    var cookieValue = "".concat(cookieName, "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;");
    document.cookie += cookieValue;
}
//# sourceMappingURL=cookieManager.js.map