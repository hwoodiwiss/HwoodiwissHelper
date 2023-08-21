namespace HwoodiwissUpdater;

public static class ApplicationMetadata
{
    // Can't use const as otherwise we get warnings about unreachable code
    public static bool IsNativeAot =>
#if NativeAot
            true;
#else
            false;
#endif
}
