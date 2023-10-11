namespace PinBlock.Tests;

public static class PinBlockTestData
{
    public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                //  key, pin, pan, pinBlock
                new[] { "0123456789ABCDEFFEDCBA9876543210", "1234", "7777770000075101538", "81C2C3AF6CA221A5" },
                new[] { "98F849D580E001BF23B5834C16436B6B", "1313", "0000100001899846", "60D99AF77B9A6DC7" },
            };
}