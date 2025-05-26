using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Restorator.Desktop.Behaviors
{
    public class DynamicScrollViewerLazyLoadBehavior : Behavior<DynamicScrollViewer>
    {
        public static readonly DependencyProperty ScrollToEndCommandProperty =
        DependencyProperty.Register("LoadDataCommand", typeof(System.Windows.Input.ICommand), typeof(DynamicScrollViewerLazyLoadBehavior));

        public System.Windows.Input.ICommand LoadDataCommand
        {
            get { return (System.Windows.Input.ICommand)GetValue(ScrollToEndCommandProperty); }
            set { SetValue(ScrollToEndCommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is DynamicScrollViewer scrollViewer)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            if (AssociatedObject != null)
            {
                AssociatedObject.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is DynamicScrollViewer scrollViewer)
            {
                if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                {
                    if (LoadDataCommand != null && LoadDataCommand.CanExecute(null))
                    {
                        LoadDataCommand.Execute(null);
                    }
                }
            }
        }
    }
}