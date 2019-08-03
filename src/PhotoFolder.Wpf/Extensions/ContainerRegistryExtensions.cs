using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Prism.Ioc;
using System;
using System.Linq;

namespace PhotoFolder.Wpf.Extensions
{
    public static class ContainerRegistryExtensions
    {
        public static void Populate(this IContainerRegistry container, IContainer autofac)
        {
            foreach (var registration in autofac.ComponentRegistry.Registrations)
            {
                if (registration.Activator.LimitType == typeof(Autofac.Core.Lifetime.LifetimeScope)) continue;

                Action<Type> register;
                if (registration.Lifetime.GetType() == typeof(CurrentScopeLifetime))
                {
                    register = type => container.Register(type, registration.Activator.LimitType);
                }
                else
                {
                    register = type => container.RegisterSingleton(type, registration.Activator.LimitType);
                }

                foreach (var service in registration.Services.OfType<IServiceWithType>())
                {
                    try
                    {
                        register(service.ServiceType);
                    }
                    catch (Exception)
                    {
                        container.RegisterInstance(service.ServiceType,
                            registration.Activator.ActivateInstance(autofac, new Parameter[0]));
                    }
                }
            }
        }
    }
}
