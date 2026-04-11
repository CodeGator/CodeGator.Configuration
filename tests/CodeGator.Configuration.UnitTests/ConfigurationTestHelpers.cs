namespace CodeGator.Configuration.UnitTests;

internal static class ConfigurationTestHelpers
{
    public static IConfiguration Build(params KeyValuePair<string, string?>[] pairs) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(pairs))
            .Build();
}
