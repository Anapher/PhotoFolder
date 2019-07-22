using Autofac;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;

namespace PhotoFolder.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IRepository<>)).AsImplementedInterfaces();
        }
    }
}
