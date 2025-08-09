using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT7SKU.Hosting.EveFrontierEVEWorld
{
    internal class EveWorldLifecycleHook : IDistributedApplicationLifecycleHook, IAsyncDisposable
    {
        private readonly ResourceNotificationService _notificationService;

        private readonly CancellationTokenSource _tokenSource = new();

        public EveWorldLifecycleHook(ResourceNotificationService notificationService, CancellationTokenSource tokenSource = default)
        {
            _notificationService = notificationService;
        }
        public Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var resource in appModel.Resources.OfType<EveWorldResource>())
            {
                //DownloadModel(resource, _tokenSource.Token);
            }

            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            _tokenSource.Cancel();
            return default;
        }
    }
}
