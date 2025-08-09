using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspire.Hosting.ApplicationModel
{ 
    public static class EveWorldResourceExtensions
    {
        /// <summary>
        /// Adds the <see cref="EveWorldResource"/> to given
        /// <paramref name="builder"/> instance. Uses the "Latest" tag
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="port"></param>
        /// <returns>
        /// An <see cref="IResourceBuilder{EveWorldResource}"/> instance that
        /// represent the added EveWorldResource
        /// </returns>
        public static IResourceBuilder<EveWorldResource> AddEveWorld(this IDistributedApplicationBuilder builder,
            string name, int? port, string privatekey)
        {
            var resource = new EveWorldResource(name);
            List<string> env = ["--block-time", "1", "--block-base-fee-per-gas","0", "--gas-limit", "3000000000", "--hardfork", "cancun", "--host", "0.0.0.0", "--port", "8546"];
            List<string> cmd = ["--rpc-url", resource.ConnectionStringExpression.ToString(), "--private-key", privatekey, "-it", "world-deployer", "deploy-v2"];

            var root = Directory.GetCurrentDirectory();
            string EnvfilePath = Path.Combine(root,".env");
            return builder.AddResource(resource)
                .WithImage(EveWorldContainerImageTags.Image)
                .WithImageRegistry(EveWorldContainerImageTags.Registry)
                .WithImageTag(EveWorldContainerImageTags.Tag)
                .WithDockerfile("world")
                .WithHttpEndpoint(targetPort: 8545, port: port)
                .WithVolume("frontier", "/root/.foundry/bin").WithContainerRuntimeArgs("--workdir", "/monorepo")
                .WithArgs(context =>
                {
                    context.Args.Add("-c");
                    context.Args.Add(string.Join(' ',cmd));
                }).WithEntrypoint("/bin/sh")
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
    internal static class EveWorldContainerImageTags
    {
        internal const string Registry = "ghcr.io";
        internal const string Image = "projectawakening/world-chain-deployer-image";
        internal const string Tag = "latest";
    }
}
