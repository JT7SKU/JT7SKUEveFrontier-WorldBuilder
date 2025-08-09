using System;
namespace Aspire.Hosting.ApplicationModel;

public sealed class EveWorldResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
    internal const string RPCEndpointName = "rpc";
    internal const string HttpEndpointName = "http";
    internal const string EveWorldEndpointName = "world";
    private EndpointReference? endpointReference;

    
    public EndpointReference Endpoint =>
       endpointReference ??= new(this, RPCEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
       ReferenceExpression.Create(
           $"http://{Endpoint.Property(EndpointProperty.HostAndPort)}"
       );
}
