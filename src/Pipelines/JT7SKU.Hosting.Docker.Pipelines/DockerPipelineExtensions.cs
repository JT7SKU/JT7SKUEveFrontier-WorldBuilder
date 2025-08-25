using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Docker;

namespace Aspire.Hosting
{
    public static class DockerPipelineExtensions
    {
        public static IResourceBuilder<DockerComposeEnvironmentResource> WithSshDeploySupport(
      this IResourceBuilder<DockerComposeEnvironmentResource> resourceBuilder)
        {
            // REVIEW: This needs to be disposed...
            var pipelineResource = new DockerSSHPipeline();
#pragma warning disable ASPIREPUBLISHERS001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            return resourceBuilder.WithAnnotation(new DeployingCallbackAnnotation(pipelineResource.Deploy));
#pragma warning restore ASPIREPUBLISHERS001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }
    }
}
