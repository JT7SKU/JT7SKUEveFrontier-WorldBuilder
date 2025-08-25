internal class Apphost
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        // This include Indexers 
        var stillnessWorldaddress = "0x7085f3e652987f656fb8dee5aa6592197bb75de8 ";
        var EFIndexer = builder.AddDockerComposeEnvironment("inxdexer-env").WithDashboard(dash => dash.WithHostPort(8085));
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