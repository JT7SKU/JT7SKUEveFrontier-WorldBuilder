internal class Apphost
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        // This include Indexers 
        var stillnessWorldaddress = "0x42abe708fe7a003769d77cca42affb9a8feb44db";
        var EFIndexer = builder.AddDockerComposeEnvironment("inxdexer-env");
        EFIndexer.ConfigureComposeFile(file =>
        {
            file.Name = "Frontier-indexer";
        });

        var pg = builder.AddPostgres("pg")
           .WithPgWeb();
        var hardhatDB = pg.AddDatabase("hardhatdb");
        var pyropeDB = pg.AddDatabase("pyropedb");
        var worldapi = builder.AddExternalService("world-api", "https://world-api-stillness.live.tech.evefrontier.com");
        var pyropeIndexer = builder.AddExternalService("Pyrope-indexer", "https://pyrope-external-sync-node-rpc.live.tech.evefrontier.com");



        builder.Build().Run();
    }
}