using Autofac;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Services;
using System.Collections.Generic;
using PhotoFolder.Core.Interfaces.Services;

namespace PhotoFolder.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileContentInfoComparer>().As<IEqualityComparer<IFileContentInfo>>();
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IUseCaseRequestHandler<,>)).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IFileIntegrityValidator>().As<IFileIntegrityValidator>().SingleInstance();
        }
    }
}
