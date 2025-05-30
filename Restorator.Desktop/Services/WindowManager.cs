using Microsoft.Extensions.DependencyInjection;
using Restorator.Desktop.ViewModels.Abstract;

namespace Restorator.Desktop.Services
{
    public interface IDialog<TViewmodel> : IDialog where TViewmodel : ViewModelBase
    { }

    public interface IDialog
    {
        bool? ShowDialog();
    }

    public interface IWindowManager
    {
        public bool ShowWindow<T>() where T : ViewModelBase;
    }

    public class WindowManager : IWindowManager
    {
        private readonly IServiceProvider _serviceProvider;
        public WindowManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool ShowWindow<TViewmodel>() where TViewmodel : ViewModelBase
        {
            var dialog = _serviceProvider.GetRequiredService<IDialog<TViewmodel>>();

            return dialog.ShowDialog() == true;
        }
    }
}