namespace PinBlock.Tests;

public class PinBlockTest
{
    [Theory]
    [MemberData(nameof(PinBlockTestData.Data), MemberType = typeof(PinBlockTestData))]
    public void Test_Encode(string key, string pin, string pan, string pinBlock)
    {
        using PinBlockEncoder encoder = new(key);
        string result = encoder.Encode(pan, pin);

        Assert.Equal(result, pinBlock);
    }

    [Theory]
    [MemberData(nameof(PinBlockTestData.Data), MemberType = typeof(PinBlockTestData))]
    public void Test_Decode(string key, string pin, string pan, string pinBlock)
    {
        using PinBlockDecoder decoder = new(key);
        string result = decoder.Decode(pinBlock, pan);

        Assert.Equal(result, pin);
    }
}