using Autofac;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Services;

namespace PhotoFolder.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IUseCaseRequestHandler<,>)).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IFileIntegrityValidator>().As<IFileIntegrityValidator>().SingleInstance();
        }
    }
}
