using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT7SKU.Hosting.EveFrontierWorldDeployer
{
    internal class WorldDeployerResourceLifecycleHook : IDistributedApplicationLifecycleHook, IAsyncDisposable
    {
        private readonly ResourceNotificationService _notificationService;

        private readonly CancellationTokenSource _tokenSource = new();

        public WorldDeployerResourceLifecycleHook(ResourceNotificationService notificationService, CancellationTokenSource tokenSource = default)
        {
            _notificationService = notificationService;
        }

        public Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var resource in appModel.Resources.OfType<WorldDeployerResource>())
            {
                //DownloadModel(resource, _tokenSource.Token);
            }

            return Task.CompletedTask;
        }
        private async Task<bool> HasFoundryUp(RpcClient rpcClient, string connectionstring, CancellationToken token)
        {
            rpcClient = new RpcClient(new Uri(connectionstring));
            return false;
        }
        //private void StartTrackingFoundryContainerLogs(FoundryResource resource, CancellationToken cancellationToken)
        //{
        //    var logger = loggerService.GetLogger(resource);

        //    _ = Task.Run(async () =>
        //    {
        //        var cmd = Cli.Wrap("docker").WithArguments(["logs", "--follow", resource.Name]);
        //        var cmdEvents = cmd.ListenAsync(cancellationToken);

        //        await foreach (var cmdEvent in cmdEvents)
        //        {
        //            switch (cmdEvent)
        //            {
        //                case StartedCommandEvent:
        //                    await notificationService.PublishUpdateAsync(resource, state => state with { State = "Running" });
        //                    break;
        //                case ExitedCommandEvent:
        //                    await notificationService.PublishUpdateAsync(resource, state => state with { State = "Finished" });
        //                    break;
        //                case StandardOutputCommandEvent stdOut:
        //                    logger.LogInformation("World-Deployer container {ResourceName} stdout: {StdOut}", resource.Name, stdOut.Text);
        //                    break;
        //                case StandardErrorCommandEvent stdErr:
        //                    logger.LogInformation("External container {ResourceName} stderr: {StdErr}", resource.Name, stdErr.Text);
        //                    break;
        //            }
        //        }
        //    }, cancellationToken);
        //}
        public ValueTask DisposeAsync()
        {
            _tokenSource.Cancel();
            return default;
        }
    }
}
