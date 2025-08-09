
using Aspire.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using Aspire.Hosting.Docker.Resources.ServiceNodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Aspire.Hosting.ApplicationModel;
using System.Xml.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using Nethereum.Web3;
using Nethereum.Model;
using Microsoft.AspNetCore.Routing.Matching;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);
        //var foundryCheck = new FoundryHealthCheck();
        var interval = TimeSpan.FromSeconds(5);
        var startPeriod = DateTime.Now.AddSeconds(20);
        builder.Services.AddHealthChecks().AddCheck("FoundryCheck", () =>
        {
            return DateAndTime.Now > startPeriod ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
;        } );
        
        var worldDeployerImg = "ghcr.io/projectawakening/world-chain-deployer-image";
        var foundryImg = "ghcr.io/foundry-rs/foundry";
       
        var TESTPrivateKey = builder.AddParameter("PrivateKey");
        var root = Directory.GetCurrentDirectory();
        string EnvfilePath = Path.Combine(root, "world.env");
        //var konttifilu = "world-deployer:/monorepo/abis";
        var dockercompose = builder.AddDockerComposeEnvironment("env");
        dockercompose.ConfigureComposeFile(file =>
        {
            file.Name = "frontier-world-builder";
        });

        //dockercompose.WithDashboard(db => db.WithHostPort(8035)).ConfigureComposefile(file=>
        //{
        //    file.Name = "frontier-world-builder";
        //});
        //var web3 = new Web3( $"{foundry.GetEndpoint("http")}");
        #region Hardhat 

        var pg = builder.AddPostgres("pg")
            .WithPgWeb();
        var pgdb = pg.AddDatabase("pgdb");

        var foundry = builder.AddContainer(name:"foundry", "ghcr.io/foundry-rs/foundry")
            .WithImage(foundryImg).WithImageTag("latest")
            .WithVolume("frontier", "/root/.foundry/bin")
            .WithEndpoint("http", e=> e.TargetHost = "0.0.0.0")
            .WithHttpEndpoint(name:"rpc",port:8546,targetPort: 8546, isProxied: false)
                .WithEntrypoint("anvil") // if still works use "/bin/sh"
                .WithEnvironment(" --block-time", "1")
                .WithEnvironment("--block-base-fee-per-gas", "0")
                .WithEnvironment("--gas-limit", "3000000000")
                .WithEnvironment("--hardfork", "cancun")
                .WithEnvironment("--host", "0.0.0.0")
                .WithHealthCheck("FoundryCheck")
                .WithExternalHttpEndpoints()
                .WithLifetime(ContainerLifetime.Persistent);
        var localUrl = foundry.GetEndpoint("http");
        var hardhat = new Web3(localUrl.ToString());
        
        var worldDeployer = builder.AddContainer(name: "world-deployer", image:"ghcr.io/projectawakening/world-chain-deployer-image")
                .WithImage(worldDeployerImg).WithImageTag("0.1.6")
                .WithContainerRuntimeArgs("--workdir", "/monorepo")
                .WithArgs("deploy-v2")
                .WithArgs("--rpc-url", localUrl)
                .WithArgs("--private-key", TESTPrivateKey)
                .WithExternalHttpEndpoints()
                .WithEnvironment(contex =>
                {
                    if (File.Exists(EnvfilePath)) return;
                    foreach (var line in File.ReadAllLines(EnvfilePath))
                    {
                        var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length != 2) continue;

                        contex.EnvironmentVariables[parts[0]] = parts[1];
                    }
                })
                .WaitFor(foundry);

        foundry.WithUrl("https://getfoundry.sh/", "Blazing fast smart contract development toolkit")
            .WithUrl("https://github.com/foundry-rs/foundry","Foundry-RS repo").WithUrlForEndpoint("htts", u => u.DisplayText = "Website"); 
        worldDeployer.WithUrl("https://evefrontier.com/en", "Eve Frontier website")
            .WithUrl("https://github.com/projectawakening", "frontier repo").WithUrlForEndpoint("htts",u => u.DisplayText="Website");
        // get abis after world-deployer have done it work for 
        // You can also retrieve the world abis and save them to the root directory from the deployment by running:
        //var abis = worldDeployer.WithDockerfile("/monorepo/abis");
        #endregion
        // frontEnd with pnpm if set 
        //builder.AddPnpmApp("eve-frontier", workingDirectory: "/Dapp").WaitFor(worldDeployer);





        builder.Build().Run();
    }
}