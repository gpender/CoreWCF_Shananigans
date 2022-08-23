using System;
using System.Collections.ObjectModel;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CoreWCFServiceNet6.guy
{
    public class DependencyInjectionServiceBehavior : IServiceBehavior
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectionServiceBehavior(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger>();
            var instanceProvider = new DependencyInjectionInstanceProvider(_serviceProvider, serviceDescription.ServiceType, logger);

            foreach (var dispatcher in serviceHostBase.ChannelDispatchers)
            {
                if (dispatcher is ChannelDispatcher channelDispatcher)
                {
                    foreach (var endpointDispatcher in channelDispatcher.Endpoints)
                    {
                        endpointDispatcher.DispatchRuntime.InstanceProvider = instanceProvider;
                    }
                }
            }
        }
    }

}