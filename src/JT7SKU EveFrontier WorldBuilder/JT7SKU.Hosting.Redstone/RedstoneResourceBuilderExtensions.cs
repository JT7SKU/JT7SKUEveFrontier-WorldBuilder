using Aspire.Hosting.ApplicationModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspire.Hosting
{
    public static class RedstoneResourceBuilderExtensions
    {
        public static IResourceBuilder<RedstoneResource> AddOPRedstone(
            this IDistributedApplicationBuilder builder, string name)
        {
            var resource = new RedstoneResource(name);

            return builder.AddResource(resource);
        }
    }

    public static class RedstoneContainerImageTags
    {
        internal const string Registry = "us-docker.pkg.dev";

        internal const string OpNodeImage = "oplabs-tools-artifacts/images/op-node";
        internal const string OPGethImage = "oplabs-tools-artifacts/images/op-geth";

        internal const string OPGethTag = "v1.101315.2";
        internal const string OPNodeTag = "v1.12.1";
    }
}
