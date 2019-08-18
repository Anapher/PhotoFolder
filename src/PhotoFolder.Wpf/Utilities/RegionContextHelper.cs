using System;
using System.Windows.Controls;
using Prism.Regions;

namespace PhotoFolder.Wpf.Utilities
{
    public static class RegionContextHelper
    {
        public static void UseRegionContext<TViewModel, TRegionContext>(this UserControl userControl,
            Action<TViewModel, TRegionContext> initializeAction)
        {
            var regionContext = RegionContext.GetObservableContext(userControl);

            void UpdateRegionContext()
            {
                if (regionContext.Value == null) return;
                if (userControl.DataContext == null) return;

                var context = (TRegionContext) regionContext.Value;
                var viewModel = (TViewModel) userControl.DataContext;
                initializeAction(viewModel, context);
            }

            regionContext.PropertyChanged += (_, __) => UpdateRegionContext();
            userControl.DataContextChanged += (_, __) => UpdateRegionContext();
        }
    }
}