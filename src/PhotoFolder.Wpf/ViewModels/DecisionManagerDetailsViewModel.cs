﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels.DecisionManager;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionManagerDetailsViewModel : BindableBase
    {
        private DecisionManagerContext? _decisionContext;
        private IssueDecisionWrapperViewModel? _selection;
        private BitmapSource? _thumbnail;
        private DelegateCommand<IssueDecisionWrapperViewModel>? _openFileCommand;

        public IssueDecisionWrapperViewModel? Selection
        {
            get => _selection;
            set => SetProperty(ref _selection, value);
        }

        public BitmapSource? Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }

        public DecisionManagerContext? DecisionContext
        {
            get => _decisionContext;
            private set => SetProperty(ref _decisionContext, value);
        }

        public DelegateCommand<IssueDecisionWrapperViewModel> OpenFileCommand
        {
            get
            {
                return _openFileCommand ??= new DelegateCommand<IssueDecisionWrapperViewModel>(parameter =>
                {
                    if (_decisionContext == null) throw new InvalidOperationException();

                    var absolutePath = parameter.Decision.Issue.File.Filename;
                    Process.Start(new ProcessStartInfo(absolutePath) { UseShellExecute = true });
                });
            }
        }

        public void Initialize(DecisionManagerContext context)
        {
            DecisionContext = context;
            context.PropertyChanged += ContextOnPropertyChanged;
            OnSelectedDecisionChanged(context.SelectedIssue);
        }

        private void ContextOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DecisionManagerContext.SelectedIssue))
            {
                var context = (DecisionManagerContext) sender;
                OnSelectedDecisionChanged(context.SelectedIssue);
            }
        }

        public async void OnSelectedDecisionChanged(IssueDecisionWrapperViewModel? viewModel)
        {
            Selection = viewModel;

            if (viewModel == null)
                Thumbnail = null;
            else
            {
                if (DecisionContext == null) return;

                var absolutePath = viewModel.Decision.Issue.File.Filename;
                var image = await ImageUtils.LoadImageAsync(absolutePath);

                if (Selection == viewModel)
                    Thumbnail = image;
            }
        }
    }
}