namespace CodeGator.Configuration.UnitTests;

[TestClass]
public sealed class ConfigurationExtensionsFieldTests
{
    [TestMethod]
    public void FieldIsMissing_ReturnsTrue_WhenKeyAndIndexedKeyAbsent()
    {
        var config = ConfigurationTestHelpers.Build();

        Assert.IsTrue(config.FieldIsMissing("Missing"));
    }

    [TestMethod]
    public void FieldIsMissing_ReturnsFalse_WhenScalarValuePresent()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Name", "value"));

        Assert.IsFalse(config.FieldIsMissing("Name"));
    }

    [TestMethod]
    public void FieldIsMissing_ReturnsFalse_WhenArrayIndexZeroPresent()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Items:0", "a"));

        Assert.IsFalse(config.FieldIsMissing("Items"));
    }

    [TestMethod]
    public void FieldIsArray_ReturnsTrue_WhenIndexZeroExists()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Items:0", "x"));

        Assert.IsTrue(config.FieldIsArray("Items"));
    }

    [TestMethod]
    public void FieldIsArray_ReturnsFalse_WhenOnlyScalarExists()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Items", "single"));

        Assert.IsFalse(config.FieldIsArray("Items"));
    }
}
