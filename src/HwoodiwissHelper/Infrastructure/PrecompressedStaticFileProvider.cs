using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace HwoodiwissHelper.Infrastructure;

public sealed class PrecompressedStaticFileProvider(IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor) : IFileProvider
{
    private readonly IFileProvider _fileProvider = hostingEnvironment.WebRootFileProvider;

    public IDirectoryContents GetDirectoryContents(string subpath)
        => _fileProvider.GetDirectoryContents(subpath);

    public IFileInfo GetFileInfo(string subpath)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);
        if (httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Accept-Encoding", out var encodings))
        {

            if (encodings.Any(static encoding => encoding?.Contains("br") == true))
            {
                var compressedAsset = _fileProvider.GetFileInfo(subpath + ".br");
                if (compressedAsset.Exists)
                {
                    return compressedAsset;
                }
            }

            if (encodings.Any(static encoding => encoding?.Contains("gzip") == true))
            {
                var compressedEncoding = _fileProvider.GetFileInfo(subpath + ".gz");
                if (compressedEncoding.Exists)
                {
                    return compressedEncoding;
                }
            }
        }

        return _fileProvider.GetFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
        => _fileProvider.Watch(filter);
}
