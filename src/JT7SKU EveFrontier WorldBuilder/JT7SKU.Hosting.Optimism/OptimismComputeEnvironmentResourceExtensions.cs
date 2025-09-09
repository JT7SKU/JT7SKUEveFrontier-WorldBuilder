using Aspire.Hosting.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspire.Hosting
{
    [Experimental("ASPIRECOMPUTE001")]
    public static class OptimismComputeEnvironmentResourceExtensions
    {
        public static IResourceBuilder<OptimismComputeEnvironmentResource> AddComputeEnvironment(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name)
        {
            var resource = new OptimismComputeEnvironmentResource(name);

            return builder.AddResource(resource);
        }
    }
}
