namespace Aspire.Hosting.ApplicationModel {
    public sealed class RedstoneResource(string name) : ContainerResource(name), IResourceWithConnectionString
    {
     
        public ReferenceExpression ConnectionStringExpression => throw new NotImplementedException();
    }
}
