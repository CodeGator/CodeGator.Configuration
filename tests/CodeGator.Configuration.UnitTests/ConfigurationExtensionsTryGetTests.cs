namespace CodeGator.Configuration.UnitTests;

[TestClass]
public sealed class ConfigurationExtensionsTryGetTests
{
    [TestMethod]
    [DataRow("true", true)]
    [DataRow("false", false)]
    public void TryGetAsBoolean_ParsesValidStrings(string raw, bool expected)
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Flag", raw));

        var ok = config.TryGetAsBoolean("Flag", out var value);

        Assert.IsTrue(ok);
        Assert.AreEqual(expected, value);
    }

    [TestMethod]
    public void TryGetAsBoolean_ReturnsFalse_WhenMissing()
    {
        var config = ConfigurationTestHelpers.Build();

        var ok = config.TryGetAsBoolean("Flag", out var value);

        Assert.IsFalse(ok);
        Assert.AreEqual(default, value);
    }

    [TestMethod]
    public void TryGetAsBoolean_ReturnsFalse_WhenInvalid()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Flag", "not-a-bool"));

        var ok = config.TryGetAsBoolean("Flag", out _);

        Assert.IsFalse(ok);
    }

    [TestMethod]
    public void TryGetAsInt_ParsesAndFailsAppropriately()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("N", "42"));

        Assert.IsTrue(config.TryGetAsInt("N", out var n));
        Assert.AreEqual(42, n);
        Assert.IsFalse(config.TryGetAsInt("Missing", out _));
    }

    [TestMethod]
    public void TryGetAsDouble_ParsesValue()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("X", "3.14"));

        Assert.IsTrue(config.TryGetAsDouble("X", out var x));
        Assert.AreEqual(3.14, x, 0.0001);
    }

    [TestMethod]
    public void TryGetAsGuid_ParsesGuidString()
    {
        var id = Guid.NewGuid();
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Id", id.ToString()));

        Assert.IsTrue(config.TryGetAsGuid("Id", out var parsed));
        Assert.AreEqual(id, parsed);
    }

    [TestMethod]
    public void TryGetAsTimeSpan_ParsesStandardFormat()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Ts", "01:02:03"));

        Assert.IsTrue(config.TryGetAsTimeSpan("Ts", out var ts));
        Assert.AreEqual(new TimeSpan(1, 2, 3), ts);
    }

    [TestMethod]
    public void TryGetAsList_ReadsIndexedKeys()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Items:0", "a"),
            new KeyValuePair<string, string?>("Items:1", "b"));

        var ok = config.TryGetAsList<string>("Items", out var list);

        Assert.IsTrue(ok);
        CollectionAssert.AreEqual(new[] { "a", "b" }, list.ToArray());
    }

    [TestMethod]
    public void TryGetAsList_ReturnsFalse_WhenEmpty()
    {
        var config = ConfigurationTestHelpers.Build();

        var ok = config.TryGetAsList<string>("Items", out var list);

        Assert.IsFalse(ok);
        Assert.AreEqual(0, list.Count());
    }

    [TestMethod]
    public void TryGetAs_Int_ReturnsParsedValue()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("N", "7"));

        var ok = config.TryGetAs("N", out int value);

        Assert.IsTrue(ok);
        Assert.AreEqual(7, value);
    }

    [TestMethod]
    public void TryGetAs_String_ReturnsParsedValue()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("S", "hello"));

        var ok = config.TryGetAs("S", out string? value);

        Assert.IsTrue(ok);
        Assert.AreEqual("hello", value);
    }

    private enum SampleEnum
    {
        None = 0,
        One = 1,
    }

    [TestMethod]
    public void TryGetAs_Enum_ParsesName()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("E", nameof(SampleEnum.One)));

        var ok = config.TryGetAs("E", out SampleEnum value);

        Assert.IsTrue(ok);
        Assert.AreEqual(SampleEnum.One, value);
    }
}
