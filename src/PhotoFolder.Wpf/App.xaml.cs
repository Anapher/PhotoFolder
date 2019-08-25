using System;
using Autofac;
using Microsoft.Extensions.Options;
using PhotoFolder.Application;
using PhotoFolder.Core;
using PhotoFolder.Infrastructure;
using PhotoFolder.Infrastructure.Photos;
using PhotoFolder.Wpf.Extensions;
using PhotoFolder.Wpf.Services;
using Prism.Ioc;
using Prism.Modularity;
using System.IO.Abstractions;
using System.Windows;
using DryIoc;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Services.FileIntegrityValidators;
using Prism.DryIoc;

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

            builder.RegisterInstance(Options.Create(new WorkspaceOptions())).As<IOptions<WorkspaceOptions>>();
            builder.RegisterInstance(Options.Create(new BitmapHashOptions())).As<IOptions<BitmapHashOptions>>();

            var container = builder.Build();
            containerRegistry.Populate(container);

            containerRegistry.Register<IWindowService, WindowService>();
            containerRegistry.RegisterDialogWindow<DialogWindow>();
            containerRegistry.RegisterInstance<IFileSystem>(new FileSystem());
            containerRegistry.RegisterSingleton<IAppSettingsProvider, AppSettingsProvider>();

            var drylocContainer = containerRegistry.GetContainer();
            drylocContainer.Register<IFileIntegrityValidator, DuplicateFileIntegrityValidator>(Reuse.Singleton, null, null,
                IfAlreadyRegistered.AppendNewImplementation);
            drylocContainer.Register<IFileIntegrityValidator, SimilarFileIntegrityValidator>(Reuse.Singleton, null, null,
                IfAlreadyRegistered.AppendNewImplementation);
            drylocContainer.Register<IFileIntegrityValidator, InvalidLocationFileIntegrityValidator>(Reuse.Singleton, null, null,
                IfAlreadyRegistered.AppendNewImplementation);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<AppViewModule>();
        }
    }
}
