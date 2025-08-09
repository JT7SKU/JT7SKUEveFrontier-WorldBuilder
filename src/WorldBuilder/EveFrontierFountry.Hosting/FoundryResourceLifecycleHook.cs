using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT7SKU.Hosting.Foundry
{
    internal class FoundryResourceLifecycleHook : IDistributedApplicationLifecycleHook, IAsyncDisposable
    {
        private readonly ResourceNotificationService _notificationService;

        private readonly CancellationTokenSource _tokenSource = new();
        private ILogger loggerService;

        public FoundryResourceLifecycleHook(ResourceNotificationService notificationService, CancellationTokenSource tokenSource = default)
        {
            _notificationService = notificationService;
        }
        public Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var resource in appModel.Resources.OfType<FoundryResource>())
            {
                //DownloadModel(resource, _tokenSource.Token);
            }

            return Task.CompletedTask;
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
        //                    logger.LogInformation("External container {ResourceName} stdout: {StdOut}", resource.Name, stdOut.Text);
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
