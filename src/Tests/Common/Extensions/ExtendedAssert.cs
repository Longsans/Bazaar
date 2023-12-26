namespace TestingCommon.Extensions;

public static class ExtendedAssert
{
    public static void SameTime(DateTime first, DateTime second)
    {
        Assert.True(Math.Abs((first - second).TotalSeconds) < TimeSpan.FromSeconds(1).TotalSeconds);
    }
}
