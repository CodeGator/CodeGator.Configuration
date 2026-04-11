namespace CodeGator.Configuration.UnitTests;

[TestClass]
public sealed class ConfigurationExtensionsHierarchyTests
{
    [TestMethod]
    public void HasChildren_ReturnsFalse_ForLeafSection()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("A", "1"));

        Assert.IsFalse(config.GetSection("A").HasChildren());
    }

    [TestMethod]
    public void HasChildren_ReturnsTrue_WhenNestedKeysExist()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("A:B", "1"));

        Assert.IsTrue(config.GetSection("A").HasChildren());
    }

    [TestMethod]
    public void GetPath_ReturnsPathForSection()
    {
        var root = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Section:Key", "v"));
        var section = root.GetSection("Section");

        Assert.AreEqual("Section", section.GetPath());
    }

    [TestMethod]
    public void GetPath_AndLeafValue_AreSetForNestedKey()
    {
        var root = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Section:Key", "expected"));
        var leaf = root.GetSection("Section:Key");

        Assert.AreEqual("Section:Key", leaf.GetPath());
        Assert.AreEqual("expected", leaf.Value);
    }

    [TestMethod]
    public void GetValue_MatchesGetSectionWithEmptyKey()
    {
        var root = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Section:Key", "expected"));
        var leaf = root.GetSection("Section:Key");

        Assert.AreEqual(leaf.GetSection("")?.Value ?? "", leaf.GetValue());
    }

    [TestMethod]
    public void GetRoot_ReturnsConfigurationRoot()
    {
        var root = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("A:B", "x"));
        var nested = root.GetSection("A");

        Assert.AreSame(root, nested.GetRoot());
    }

    [TestMethod]
    public void GetParentSection_ReturnsParent_ForNestedSection()
    {
        var root = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Outer:Inner:Leaf", "1"));
        var inner = root.GetSection("Outer:Inner");

        var parent = inner.GetParentSection();

        Assert.IsNotNull(parent);
        Assert.AreEqual("Outer", parent!.Path);
    }
}
