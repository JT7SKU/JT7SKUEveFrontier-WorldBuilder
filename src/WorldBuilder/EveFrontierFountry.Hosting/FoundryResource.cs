// For ease of discovery, resource types should be placed in
// the Aspire.Hosting.ApplicationModel namespace. If there is
// likelihood of a conflict on the resource name consider using
// an alternative namespace.
using Aspire.Hosting.Docker;
using Aspire.Hosting.Docker.Resources.ComposeNodes;
using Aspire.Hosting.Docker.Resources.ServiceNodes;


namespace Aspire.Hosting.ApplicationModel;

public sealed class FoundryResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
    // Constants used to refer to well known-endpoint names, this is specific
    // for each resource type. Foundry exposes an rpc endpoint and a HTTP
    // endpoint.
    internal const string RPCEndpointName = "rpc";
    internal const string HttpEndpointName = "http";

  

    // An EndpointReference is a core .NET Aspire type used for keeping
    // track of endpoint details in expressions. Simple literal values cannot
    // be used because endpoints are not known until containers are launched.
    private EndpointReference? _rpcReference;

  
    public EndpointReference RPCEndpoint =>
        _rpcReference ??= new(this, RPCEndpointName);


    // Required property on IResourceWithConnectionString. Represents a connection
    // string that applications can use to access the Foundry server. In this case
    // the connection string is composed of the jsonRPC endpoint reference.
    
    
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"http://{RPCEndpoint.Property(EndpointProperty.HostAndPort)}"
        );
}
