namespace CodeGator.Configuration.UnitTests;

[TestClass]
public sealed class ConfigurationExtensionsGetAsTests
{
    [TestMethod]
    public void GetAsInt_ReturnsParsedOrDefault()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("A", "5"));

        Assert.AreEqual(5, config.GetAsInt("A", 9));
        Assert.AreEqual(9, config.GetAsInt("Missing", 9));
    }

    [TestMethod]
    public void GetAsBoolean_ReturnsParsedOrDefault()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("B", "true"));

        Assert.IsTrue(config.GetAsBoolean("B", false));
        Assert.IsFalse(config.GetAsBoolean("Missing", false));
    }

    [TestMethod]
    public void GetAsGuid_ReturnsParsedOrDefault()
    {
        var id = Guid.NewGuid();
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("G", id.ToString()));

        Assert.AreEqual(id, config.GetAsGuid("G", Guid.Empty));
        Assert.AreEqual(Guid.Empty, config.GetAsGuid("Missing", Guid.Empty));
    }

    [TestMethod]
    public void GetAsT_UsesDefaultWhenMissing()
    {
        var config = ConfigurationTestHelpers.Build();

        Assert.AreEqual(42, config.GetAs("N", 42));
    }
}
