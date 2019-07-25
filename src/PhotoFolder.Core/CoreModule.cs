using Autofac;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Services;
using System.Collections.Generic;

namespace PhotoFolder.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileContentInfoComparer>().As<IEqualityComparer<IFileContentInfo>>();
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IUseCaseRequestHandler<,>)).AsImplementedInterfaces();
        }
    }
}
