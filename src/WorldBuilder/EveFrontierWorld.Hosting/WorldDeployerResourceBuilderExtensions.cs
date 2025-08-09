using Aspire.Hosting.ApplicationModel;

// Put extensions in the Aspire.Hosting namespace to ease discovery as referencing
// the .NET Aspire hosting package automatically adds this namespace.
namespace Aspire.Hosting;

public static class WorldDeployerResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="WorldDeployerResource"/> to the given
    /// <paramref name="builder"/> instance. Uses the "0.0.7" tag.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="rpcurl">The rpc url.</param>
    /// <param name="privatekey">The privatekey to rpc resource.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{WorldDeployerResource}"/> instance that
    /// represents the added EveFrontierWorldDeployer resource.
    /// </returns>
    public static IResourceBuilder<WorldDeployerResource> AddEveFrontierWorldDeployer(
        this IDistributedApplicationBuilder builder,
        string name, int? port,
        string? rpcurl = null,
        string? privatekey = null)
    {
        // The AddResource method is a core API within .NET Aspire and is
        // used by resource developers to wrap a custom resource in an
        // IResourceBuilder<T> instance. Extension methods to customize
        // the resource (if any exist) target the builder interface.
        var resource = new WorldDeployerResource(name);

        var root = Directory.GetCurrentDirectory();
        string EnvfilePath = Path.Combine(root, ".env");

        return builder.AddResource(resource)
                      .WithImage(WorldDeployerContainerImageTags.Image)
                      .WithImageRegistry(WorldDeployerContainerImageTags.Registry)
                      .WithImageTag(WorldDeployerContainerImageTags.Tag)
                      .WithArgs("--rpc-url", rpcurl)
                .WithArgs("--private-key", privatekey).WithArgs("-it", "world-deployer", "deploy-v2")
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
                      .ExcludeFromManifest().PublishAsContainer();
    }

   
}


// This class just contains constant strings that can be updated periodically
// when new versions of the underlying container are released.
// the command --rpc-url http://foundry:8546 --private-key 0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80
// also depends on healthy Foundry 
internal static class WorldDeployerContainerImageTags
{
    internal const string Registry = "ghcr.io";

    internal const string Image = "projectawakening/world-chain-deployer-image";

    internal const string Tag = "0.0.7";
}
