using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Docker.Resources.ComposeNodes;
using Aspire.Hosting.Docker.Resources.ServiceNodes;
using EveFrontierFoundry.Hosting;
using Microsoft.Extensions.DependencyInjection;

// Put extensions in the Aspire.Hosting namespace to ease discovery as referencing
// the .NET Aspire hosting package automatically adds this namespace.
namespace Aspire.Hosting;

public static class FoundryResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="FoundryResource"/> to the given
    /// <paramref name="builder"/> instance. Uses the "stable" tag.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="rpcPort">The rpc port.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{FoundryResource}"/> instance that
    /// represents the added Foundry resource.
    /// </returns>
    public static IResourceBuilder<FoundryResource> AddFoundry(
        this IDistributedApplicationBuilder builder,
        string name,
        int? httpPort = null, int? rpcPort = null) // ports
    {
        // The AddResource method is a core API within .NET Aspire and is
        // used by resource developers to wrap a custom resource in an
        // IResourceBuilder<T> instance. Extension methods to customize
        // the resource (if any exist) target the builder interface.
        // entrypoint anvil --block-time 1 --block-base-fee-per-gas 0 --gas-limit 3000000000 --hardfork cancun --host 0.0.0.0 --port 8546 
        // with healthchecks: test [ "CMD-SHELL", "anvil --help || exit 1" ], interval= 10secs timeout 5secs retries 5 start period 20s
        var resource = new FoundryResource(name);
        
        return builder.AddResource(resource)
                      .WithImage(FoundryContainerImageTags.Image)
                      .WithImageRegistry(FoundryContainerImageTags.Registry)
                      .WithImageTag(FoundryContainerImageTags.Tag)
                      .WithVolume("frontier", "/root/.foundry/bin")
                      .WithHttpEndpoint(
                          targetPort: 8546,
                          port: httpPort,
                          name: FoundryResource.HttpEndpointName)
                      .WithEndpoint(
                          targetPort: 8545,
                          port: rpcPort,
                          name: FoundryResource.RPCEndpointName)
                      .WithEntrypoint( entrypoint: "/bin/sh")
                      .WithEnvironment(" --block-time", "1").WithEnvironment("--block-base-fee-per-gas", "0").WithEnvironment("--gas-limit", "3000000000")
                .WithEnvironment("--hardfork", "cancun").WithEnvironment("--host", "0.0.0.0").WithEnvironment("--port", "8546")
                      .WithHealthCheck("/healty")
                      .WithLifetime(ContainerLifetime.Persistent).PublishAsContainer();
    }
}

// This class just contains constant strings that can be updated periodically
// when new versions of the underlying container are released.
internal static class FoundryContainerImageTags
{
    internal const string Registry = "ghcr.io";

    internal const string Image = "foundry-rs/foundry";

    internal const string Tag = "stable";
}