using Microsoft.Extensions.DependencyInjection;
using Restorator.Desktop.ExceptionHandlers.Abstract;
using Restorator.Desktop.Extensions;
using Restorator.Desktop.Views.Windows;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Restorator.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly IServiceProvider _serviceProvider;
        public App()
        {
            var services = new ServiceCollection()
                .Configure();

            _serviceProvider = services.BuildServiceProvider();

            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private async void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var handlers = _serviceProvider.GetKeyedServices<ExceptionHandlerBase>(e.Exception.GetType());

            foreach (var handler in handlers)
            {
                await handler.HandleAsync(e);

                if (e.Handled)
                    return;
            }

            var snackbarService = _serviceProvider.GetRequiredService<ISnackbarService>();

            snackbarService.Show($"Произошла ошибка в {e.Exception.StackTrace}", e.Exception.Message, Wpf.Ui.Controls.ControlAppearance.Danger);

            e.Handled = true;
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            _serviceProvider.GetRequiredService<MainWindow>()
                .Show();
        }
    }
}
