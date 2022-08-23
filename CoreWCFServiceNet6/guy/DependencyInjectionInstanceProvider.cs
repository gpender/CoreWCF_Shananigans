using System;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CoreWCFServiceNet6.guy
{
    public class DependencyInjectionInstanceProvider : IInstanceProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Type _serviceType;
        private readonly ILogger _logger;

        public DependencyInjectionInstanceProvider(IServiceProvider serviceProvider, Type serviceType, ILogger logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public object GetInstance(InstanceContext instanceContext) => GetInstance(instanceContext, null);
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            if (instanceContext == null)
            {
                throw new ArgumentNullException(nameof(instanceContext));
            }
            try
            {
                var service = _serviceProvider.GetRequiredService(_serviceType);
                return service;
            }
            catch
            {
                _logger.Error("Impossible to resolve {SERVICE}", _serviceType.FullName);
                throw;
            }
        }
        public void ReleaseInstance(InstanceContext instanceContext, object instance) 
        {
            if(instance is IDisposable)
            {
                ((IDisposable)instance).Dispose();
            }
        }
    }
}