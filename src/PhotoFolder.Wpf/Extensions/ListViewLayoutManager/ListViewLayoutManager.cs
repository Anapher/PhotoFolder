// -- FILE ------------------------------------------------------------------
// name       : ListViewLayoutManager.cs
// created    : Jani Giannoudis - 2008.03.27
// language   : c#
// environment: .NET 3.0
// copyright  : (c) 2008-2012 by Itenso GmbH, Switzerland
// --------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PhotoFolder.Wpf.Extensions.ListViewLayoutManager
{
    // ------------------------------------------------------------------------
    public class ListViewLayoutManager
    {
        private const double ZeroWidthRange = 0.1;

        // ----------------------------------------------------------------------
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(ListViewLayoutManager),
            new FrameworkPropertyMetadata(OnLayoutManagerEnabledChanged));

        // ----------------------------------------------------------------------
        // members
        private GridViewColumn? _autoSizedColumn;
        private bool _loaded;
        private Cursor? _resizeCursor;
        private bool _resizing;
        private ScrollViewer? _scrollViewer;

        // ----------------------------------------------------------------------
        public ListViewLayoutManager(ListView listView)
        {
            ListView = listView ?? throw new ArgumentNullException(nameof(listView));
            ListView.Loaded += ListViewLoaded;
            ListView.Unloaded += ListViewUnloaded;
        } // ListViewLayoutManager

        // ----------------------------------------------------------------------
        public ListView ListView { get; }
        // ListView

        // ----------------------------------------------------------------------
        public ScrollBarVisibility VerticalScrollBarVisibility { get; set; } = ScrollBarVisibility.Auto;
// VerticalScrollBarVisibility

        // ----------------------------------------------------------------------
        public static void SetEnabled(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(EnabledProperty, enabled);
        } // SetEnabled

        // ----------------------------------------------------------------------
        public void Refresh()
        {
            InitColumns();
            DoResizeColumns();
        } // Refresh

        // ----------------------------------------------------------------------
        private void RegisterEvents(DependencyObject start)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                var childVisual = (Visual) VisualTreeHelper.GetChild(start, i);
                if (childVisual is Thumb thumb)
                {
                    var gridViewColumn = FindParentColumn(childVisual);
                    if (gridViewColumn != null)
                    {
                        if (ProportionalColumn.IsProportionalColumn(gridViewColumn) || FixedColumn.IsFixedColumn(gridViewColumn) ||
                            IsFillColumn(gridViewColumn))
                        {
                            thumb.IsHitTestVisible = false;
                        }
                        else
                        {
                            thumb.PreviewMouseMove += ThumbPreviewMouseMove;
                            thumb.PreviewMouseLeftButtonDown += ThumbPreviewMouseLeftButtonDown;
                            DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn))
                                .AddValueChanged(gridViewColumn, GridColumnWidthChanged);
                        }
                    }
                }
                else if (childVisual is GridViewColumnHeader columnHeader)
                {
                    columnHeader.SizeChanged += GridColumnHeaderSizeChanged;
                }
                else if (_scrollViewer == null && childVisual is ScrollViewer)
                {
                    _scrollViewer = (ScrollViewer) childVisual;
                    _scrollViewer.ScrollChanged += ScrollViewerScrollChanged;
                    // assume we do the regulation of the horizontal scrollbar
                    _scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    _scrollViewer.VerticalScrollBarVisibility = VerticalScrollBarVisibility;
                }

                RegisterEvents(childVisual); // recursive
            }
        } // RegisterEvents

        // ----------------------------------------------------------------------
        private void UnregisterEvents(DependencyObject start)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                var childVisual = (Visual) VisualTreeHelper.GetChild(start, i);
                if (childVisual is Thumb)
                {
                    var gridViewColumn = FindParentColumn(childVisual);
                    if (gridViewColumn != null)
                    {
                        var thumb = (Thumb) childVisual;
                        if (ProportionalColumn.IsProportionalColumn(gridViewColumn) || FixedColumn.IsFixedColumn(gridViewColumn) ||
                            IsFillColumn(gridViewColumn))
                        {
                            thumb.IsHitTestVisible = true;
                        }
                        else
                        {
                            thumb.PreviewMouseMove -= ThumbPreviewMouseMove;
                            thumb.PreviewMouseLeftButtonDown -= ThumbPreviewMouseLeftButtonDown;
                            DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn))
                                .RemoveValueChanged(gridViewColumn, GridColumnWidthChanged);
                        }
                    }
                }
                else if (childVisual is GridViewColumnHeader columnHeader)
                {
                    columnHeader.SizeChanged -= GridColumnHeaderSizeChanged;
                }
                else if (_scrollViewer == null && childVisual is ScrollViewer viewer)
                {
                    _scrollViewer = viewer;
                    _scrollViewer.ScrollChanged -= ScrollViewerScrollChanged;
                }

                UnregisterEvents(childVisual); // recursive
            }
        } // UnregisterEvents

        // ----------------------------------------------------------------------
        private GridViewColumn? FindParentColumn(DependencyObject element)
        {
            if (element == null) return null;

            while (element != null)
            {
                if (element is GridViewColumnHeader gridViewColumnHeader) return gridViewColumnHeader.Column;
                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        } // FindParentColumn

        // ----------------------------------------------------------------------
        private GridViewColumnHeader? FindColumnHeader(DependencyObject start, GridViewColumn gridViewColumn)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                var childVisual = (Visual) VisualTreeHelper.GetChild(start, i);
                if (childVisual is GridViewColumnHeader gridViewHeader)
                    if (gridViewHeader.Column == gridViewColumn)
                        return gridViewHeader;
                var childGridViewHeader = FindColumnHeader(childVisual, gridViewColumn); // recursive
                if (childGridViewHeader != null) return childGridViewHeader;
            }

            return null;
        } // FindColumnHeader

        // ----------------------------------------------------------------------
        private void InitColumns()
        {
            if (!(ListView.View is GridView view)) return;

            foreach (var gridViewColumn in view.Columns)
            {
                if (!RangeColumn.IsRangeColumn(gridViewColumn)) continue;

                var minWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
                var maxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);
                if (!minWidth.HasValue && !maxWidth.HasValue) continue;

                var columnHeader = FindColumnHeader(ListView, gridViewColumn);
                if (columnHeader == null) continue;

                var actualWidth = columnHeader.ActualWidth;
                if (minWidth.HasValue)
                {
                    columnHeader.MinWidth = minWidth.Value;
                    if (!double.IsInfinity(actualWidth) && actualWidth < columnHeader.MinWidth) gridViewColumn.Width = columnHeader.MinWidth;
                }

                if (maxWidth.HasValue)
                {
                    columnHeader.MaxWidth = maxWidth.Value;
                    if (!double.IsInfinity(actualWidth) && actualWidth > columnHeader.MaxWidth) gridViewColumn.Width = columnHeader.MaxWidth;
                }
            }
        } // InitColumns

        // ----------------------------------------------------------------------
        protected virtual void ResizeColumns()
        {
            if (!(ListView.View is GridView view) || view.Columns.Count == 0) return;

            // listview width
            var actualWidth = double.PositiveInfinity;
            if (_scrollViewer != null) actualWidth = _scrollViewer.ViewportWidth;
            if (double.IsInfinity(actualWidth)) actualWidth = ListView.ActualWidth;
            if (double.IsInfinity(actualWidth) || actualWidth <= 0) return;

            double resizeableRegionCount = 0;
            double otherColumnsWidth = 0;
            // determine column sizes
            foreach (var gridViewColumn in view.Columns)
                if (ProportionalColumn.IsProportionalColumn(gridViewColumn))
                {
                    var proportionalWidth = ProportionalColumn.GetProportionalWidth(gridViewColumn);
                    if (proportionalWidth != null) resizeableRegionCount += proportionalWidth.Value;
                }
                else
                {
                    otherColumnsWidth += gridViewColumn.ActualWidth;
                }

            if (resizeableRegionCount <= 0)
            {
                // no proportional columns present: commit the regulation to the scroll viewer
                if (_scrollViewer != null) _scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                // search the first fill column
                GridViewColumn? fillColumn = null;
                for (var i = 0; i < view.Columns.Count; i++)
                {
                    var gridViewColumn = view.Columns[i];
                    if (IsFillColumn(gridViewColumn))
                    {
                        fillColumn = gridViewColumn;
                        break;
                    }
                }

                if (fillColumn != null)
                {
                    var otherColumnsWithoutFillWidth = otherColumnsWidth - fillColumn.ActualWidth;
                    var fillWidth = actualWidth - otherColumnsWithoutFillWidth;
                    if (fillWidth > 0)
                    {
                        var minWidth = RangeColumn.GetRangeMinWidth(fillColumn);
                        var maxWidth = RangeColumn.GetRangeMaxWidth(fillColumn);

                        var setWidth = !(minWidth.HasValue && fillWidth < minWidth.Value);
                        if (maxWidth.HasValue && fillWidth > maxWidth.Value) setWidth = false;
                        if (setWidth)
                        {
                            if (_scrollViewer != null) _scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                            fillColumn.Width = fillWidth;
                        }
                    }
                }

                return;
            }

            var resizeableColumnsWidth = actualWidth - otherColumnsWidth;
            if (resizeableColumnsWidth <= 0) return; // missing space

            // resize columns
            var resizeableRegionWidth = resizeableColumnsWidth / resizeableRegionCount;
            foreach (var gridViewColumn in view.Columns)
                if (ProportionalColumn.IsProportionalColumn(gridViewColumn))
                {
                    var proportionalWidth = ProportionalColumn.GetProportionalWidth(gridViewColumn);
                    if (proportionalWidth != null) gridViewColumn.Width = proportionalWidth.Value * resizeableRegionWidth;
                }
        } // ResizeColumns

        // ----------------------------------------------------------------------
        // returns the delta
        private double SetRangeColumnToBounds(GridViewColumn gridViewColumn)
        {
            var startWidth = gridViewColumn.Width;

            var minWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
            var maxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);

            if (minWidth.HasValue && maxWidth.HasValue && minWidth > maxWidth) return 0; // invalid case

            if (minWidth.HasValue && gridViewColumn.Width < minWidth.Value)
                gridViewColumn.Width = minWidth.Value;
            else if (maxWidth.HasValue && gridViewColumn.Width > maxWidth.Value) gridViewColumn.Width = maxWidth.Value;

            return gridViewColumn.Width - startWidth;
        } // SetRangeColumnToBounds

        // ----------------------------------------------------------------------
        private bool IsFillColumn(GridViewColumn gridViewColumn)
        {
            if (gridViewColumn == null) return false;

            if (!(ListView.View is GridView view) || view.Columns.Count == 0) return false;

            var isFillColumn = RangeColumn.GetRangeIsFillColumn(gridViewColumn);
            return isFillColumn.HasValue && isFillColumn.Value;
        } // IsFillColumn

        // ----------------------------------------------------------------------
        private void DoResizeColumns()
        {
            if (_resizing) return;

            _resizing = true;
            try
            {
                ResizeColumns();
            }
            finally
            {
                _resizing = false;
            }
        } // DoResizeColumns

        // ----------------------------------------------------------------------
        private void ListViewLoaded(object sender, RoutedEventArgs e)
        {
            RegisterEvents(ListView);
            InitColumns();
            DoResizeColumns();

            var gridView = ListView.View as GridView;
            if (gridView != null)
                gridView.Columns.CollectionChanged += (s, args) =>
                {
                    if (args.Action != NotifyCollectionChangedAction.Move)
                        DoResizeColumns();
                };

            _loaded = true;
        } // ListViewLoaded

        // ----------------------------------------------------------------------
        private void ListViewUnloaded(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            UnregisterEvents(ListView);
            _loaded = false;
        } // ListViewUnloaded

        // ----------------------------------------------------------------------
        private void ThumbPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is Thumb thumb)) return;
            var gridViewColumn = FindParentColumn(thumb);
            if (gridViewColumn == null) return;

            // suppress column resizing for proportional, fixed and range fill columns
            if (ProportionalColumn.IsProportionalColumn(gridViewColumn) || FixedColumn.IsFixedColumn(gridViewColumn) || IsFillColumn(gridViewColumn))
            {
                thumb.Cursor = null;
                return;
            }

            // check range column bounds
            if (thumb.IsMouseCaptured && RangeColumn.IsRangeColumn(gridViewColumn))
            {
                var minWidth = RangeColumn.GetRangeMinWidth(gridViewColumn);
                var maxWidth = RangeColumn.GetRangeMaxWidth(gridViewColumn);

                if (minWidth.HasValue && maxWidth.HasValue && minWidth > maxWidth) return; // invalid case

                if (_resizeCursor == null) _resizeCursor = thumb.Cursor; // save the resize cursor

                if (minWidth.HasValue && gridViewColumn.Width <= minWidth.Value)
                    thumb.Cursor = Cursors.No;
                else if (maxWidth.HasValue && gridViewColumn.Width >= maxWidth.Value)
                    thumb.Cursor = Cursors.No;
                else
                    thumb.Cursor = _resizeCursor; // between valid min/max
            }
        } // ThumbPreviewMouseMove

        // ----------------------------------------------------------------------
        private void ThumbPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var thumb = (Thumb) sender;
            var gridViewColumn = FindParentColumn(thumb);
            if (gridViewColumn == null) throw new InvalidOperationException("Could not find parent column");

            // suppress column resizing for proportional, fixed and range fill columns
            if (ProportionalColumn.IsProportionalColumn(gridViewColumn) || FixedColumn.IsFixedColumn(gridViewColumn) || IsFillColumn(gridViewColumn))
                e.Handled = true;
        } // ThumbPreviewMouseLeftButtonDown

        // ----------------------------------------------------------------------
        private void GridColumnWidthChanged(object sender, EventArgs e)
        {
            if (!_loaded) return;

            var gridViewColumn = (GridViewColumn) sender;
            if (gridViewColumn == null) throw new ArgumentNullException(nameof(sender));

            // suppress column resizing for proportional and fixed columns
            if (ProportionalColumn.IsProportionalColumn(gridViewColumn) || FixedColumn.IsFixedColumn(gridViewColumn)) return;

            // ensure range column within the bounds
            if (RangeColumn.IsRangeColumn(gridViewColumn))
            {
                // special case: auto column width - maybe conflicts with min/max range
                if (gridViewColumn.Width.Equals(double.NaN))
                {
                    _autoSizedColumn = gridViewColumn;
                    return; // handled by the change header size event
                }

                // ensure column bounds
                if (Math.Abs(SetRangeColumnToBounds(gridViewColumn) - 0) > ZeroWidthRange) return;
            }

            DoResizeColumns();
        } // GridColumnWidthChanged

        // ----------------------------------------------------------------------
        // handle autosized column
        private void GridColumnHeaderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_autoSizedColumn == null) return;

            var gridViewColumnHeader = sender as GridViewColumnHeader;
            if (gridViewColumnHeader != null && gridViewColumnHeader.Column == _autoSizedColumn)
            {
                if (gridViewColumnHeader.Width.Equals(double.NaN))
                {
                    // sync column with 
                    gridViewColumnHeader.Column.Width = gridViewColumnHeader.ActualWidth;
                    DoResizeColumns();
                }

                _autoSizedColumn = null;
            }
        } // GridColumnHeaderSizeChanged

        // ----------------------------------------------------------------------
        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_loaded && Math.Abs(e.ViewportWidthChange - 0) > ZeroWidthRange) DoResizeColumns();
        } // ScrollViewerScrollChanged

        // ----------------------------------------------------------------------
        private static void OnLayoutManagerEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var listView = dependencyObject as ListView;
            if (listView != null)
            {
                var enabled = (bool) e.NewValue;
                if (enabled) new ListViewLayoutManager(listView);
            }
        } // OnLayoutManagerEnabledChanged
    } // class ListViewLayoutManager
} // namespace Itenso.Windows.Controls.ListViewLayout
// -- EOF -------------------------------------------------------------------
