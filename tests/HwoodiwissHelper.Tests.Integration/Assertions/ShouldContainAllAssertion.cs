namespace HwoodiwissHelper.Tests.Integration.Assertions;

public static class ShouldContainAllAssertion
{
    public static void ShouldContainAll<T, TItem>(this T actual, TItem[] expected)
        where T : IEnumerable<TItem>
    {
        foreach (var item in expected)
        {
            actual.ShouldContain(item);
        }
    }
}
