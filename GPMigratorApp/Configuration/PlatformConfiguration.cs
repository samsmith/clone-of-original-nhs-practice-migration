namespace GPMigratorApp.Configuration
{
    public sealed class PlatformConfiguration
    {
        public SqlConfiguration? Sql { get; set; }
    }

    public sealed class SqlConfiguration
    {
        public string? ReadWriteConnectionString { get; set; }
        public string? ReadOnlyConnectionString { get; set; }
    }
}
