namespace CodeGator.Configuration.UnitTests;

[TestClass]
public sealed class ConfigurationExtensionsJsonTests
{
    [TestMethod]
    public void ToJson_ContainsChildKeysAndValues()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("A", "1"),
            new KeyValuePair<string, string?>("B:Inner", "2"));

        var json = config.ToJson();

        StringAssert.Contains(json, "\"A\"");
        StringAssert.Contains(json, "\"1\"");
        StringAssert.Contains(json, "\"B\"");
    }

    [TestMethod]
    public void WriteAsJSON_WritesFileContent()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("K", "v"));
        var path = Path.Combine(Path.GetTempPath(), $"cfg-json-{Guid.NewGuid():N}.json");
        try
        {
            config.WriteAsJSON(path);

            Assert.IsTrue(File.Exists(path));
            var text = File.ReadAllText(path);
            StringAssert.Contains(text, "\"K\"");
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [TestMethod]
    public async Task WriteAsJSONAsync_WritesFileContent()
    {
        var config = ConfigurationTestHelpers.Build(
            new KeyValuePair<string, string?>("K", "v"));
        var path = Path.Combine(Path.GetTempPath(), $"cfg-json-{Guid.NewGuid():N}.json");
        try
        {
            await config.WriteAsJSONAsync(path).ConfigureAwait(false);

            Assert.IsTrue(File.Exists(path));
            var text = await File.ReadAllTextAsync(path).ConfigureAwait(false);
            StringAssert.Contains(text, "\"K\"");
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
