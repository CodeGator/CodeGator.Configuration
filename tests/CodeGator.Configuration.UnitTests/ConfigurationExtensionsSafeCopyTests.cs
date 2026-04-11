namespace CodeGator.Configuration.UnitTests;

[TestClass]
public sealed class ConfigurationExtensionsSafeCopyTests
{
    private sealed class Target
    {
        public int Count { get; set; }
        public string Name { get; set; } = "";
        public bool Flag { get; set; }
    }

    [TestMethod]
    public void SafeCopy_SetsIntProperty_WhenKeyPresent()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Count", "3"));
        var target = new Target();

        var returned = config.SafeCopy(target, t => t.Count, "Count");

        Assert.AreSame(config, returned);
        Assert.AreEqual(3, target.Count);
    }

    [TestMethod]
    public void SafeCopy_SetsStringProperty_WhenKeyPresent()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Name", "test"));
        var target = new Target();

        config.SafeCopy(target, t => t.Name, "Name");

        Assert.AreEqual("test", target.Name);
    }

    [TestMethod]
    public void SafeCopy_SetsBoolProperty_WhenKeyPresent()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("Flag", "true"));
        var target = new Target();

        config.SafeCopy(target, t => t.Flag, "Flag");

        Assert.IsTrue(target.Flag);
    }

    [TestMethod]
    public void SafeCopy_LeavesProperty_WhenKeyMissing()
    {
        var config = ConfigurationTestHelpers.Build();
        var target = new Target { Count = 7 };

        config.SafeCopy(target, t => t.Count, "Missing");

        Assert.AreEqual(7, target.Count);
    }
}
