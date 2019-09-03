using System.Collections.Generic;
using Autofac;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Infrastructure.Data;
using PhotoFolder.Infrastructure.Files;
using PhotoFolder.Infrastructure.Photos;
using PhotoFolder.Infrastructure.Serialization;
using PhotoFolder.Infrastructure.Services;

namespace PhotoFolder.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IRepository<>)).AsImplementedInterfaces();
            builder.RegisterType<JsonSerializer>().As<IDataSerializer>().SingleInstance();
            builder.RegisterType<FileInformationLoader>().AsSelf().As<IFileInformationLoader>().SingleInstance();
            builder.RegisterType<DiskLockedFileInformationLoader>().As<IFileInformationLoader>().SingleInstance();
            builder.RegisterType<SHA256FileHasher>().As<IFileHasher>().SingleInstance();
            builder.RegisterType<BitmapHashComparer>().As<IBitmapHashComparer>().SingleInstance();
            builder.RegisterType<AppDbContextOptionsBuilder>().As<IAppDbContextOptionsBuilder>().SingleInstance();

            builder.RegisterType<PhotoDirectoryCreator>().As<IPhotoDirectoryCreator>();
            builder.RegisterType<PhotoDirectoryLoader>().As<IPhotoDirectoryLoader>();

            builder.RegisterType<PathUtils>().As<IPathUtils>().SingleInstance();
            builder.RegisterType<FileBaseContextFactory>().As<IFileBaseContextFactory>().SingleInstance();
            builder.RegisterType<SimilarityDictionaryFactory>().As<ISimilarityDictionaryFactory>().SingleInstance();
            builder.RegisterType<FileContentInfoComparer>().As<IEqualityComparer<IFileContentInfo>>();
        }
    }
}
