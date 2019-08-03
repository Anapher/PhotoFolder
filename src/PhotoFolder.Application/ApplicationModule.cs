using Autofac;
using PhotoFolder.Application.Interfaces;

namespace PhotoFolder.Application
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IWorker<,,>)).AsImplementedInterfaces();
        }
    }
}
