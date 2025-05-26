using Microsoft.Extensions.DependencyInjection;
using Restorator.Desktop.Infrastructure;
using System.Windows;
using Wpf.Ui;

namespace Restorator.Desktop.Services
{
    public class PageService : IPageService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DataTemplateManager _templateManager;
        public PageService(IServiceProvider serviceProvider, DataTemplateManager templateManager)
        {
            _serviceProvider = serviceProvider;
            _templateManager = templateManager;
        }

        public T? GetPage<T>() where T : class
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException("The page should be a WPF control.");
            }

            return (T?)_serviceProvider.GetService(typeof(T));
        }

        public FrameworkElement? GetPage(Type pageType)
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
            {
                throw new InvalidOperationException("The page should be a WPF control.");
            }

            var page = (FrameworkElement)_serviceProvider.GetRequiredService(pageType);

            var viewModelType = _templateManager.TryGetRegisteredViewModel(pageType);

            if (viewModelType != null)
            {
                var viewModel = _serviceProvider.GetRequiredService(viewModelType);

                page!.DataContext = viewModel;
            }

            return page;
        }
    }
}
