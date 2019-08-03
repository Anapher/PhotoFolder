using Autofac;
using Microsoft.Extensions.Options;
using PhotoFolder.Application;
using PhotoFolder.Core;
using PhotoFolder.Infrastructure;
using PhotoFolder.Infrastructure.Photos;
using PhotoFolder.Infrastructure.Services;
using PhotoFolder.Wpf.Extensions;
using PhotoFolder.Wpf.Services;
using Prism.Ioc;
using Prism.Modularity;
using System.IO.Abstractions;
using System.Windows;

namespace PhotoFolder.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return new MainWindow();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CoreModule>();
            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterModule<ApplicationModule>();
            builder.RegisterType<FileSystem>().As<IFileSystem>();

            builder.RegisterInstance(Options.Create(new WorkspaceOptions())).As<IOptions<WorkspaceOptions>>();
            builder.RegisterInstance(Options.Create(new BitmapHashOptions())).As<IOptions<BitmapHashOptions>>();

            var container = builder.Build();
            containerRegistry.Populate(container);

            var test = container.Resolve<IPhotoDirectoryLoader>();

            containerRegistry.Register<IWindowService, WindowService>();
            containerRegistry.RegisterDialogWindow<DialogWindow>();
            containerRegistry.RegisterInstance<IFileSystem>(new FileSystem());
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<AppViewModule>();
        }
    }
}
