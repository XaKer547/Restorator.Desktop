using Microsoft.Extensions.DependencyInjection;
using Restorator.Desktop.ViewModels.Abstract;
using System.Windows.Controls;

namespace Restorator.Desktop.Services
{
    public interface INavigationService
    {
        void SetNavigationControl(Frame frame);
        void SetNavigationRoot<T>() where T : ViewModelBase;
        void ResetNavigation();

        void Navigate<T>() where T : ViewModelBase;
        void Navigate<T>(Action<T> action) where T : ViewModelBase;
        Task NavigateAsync<T>(Func<T, Task> action) where T : ViewModelBase;
        Task NavigateAsync<T>() where T : ViewModelBase;

        void NavigateWithHierarchy<T>() where T : ViewModelBase;
        void NavigateWithHierarchy<T>(Action<T> action) where T : ViewModelBase;
        Task NavigateWithHierarchyAsync<T>(Func<T, Task> action) where T : ViewModelBase;
        Task NavigateWithHierarchyAsync<T>() where T : ViewModelBase;

        void NavigateBack();
        Task NavigateBackAsync();
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private Frame _navigationControl;
        private readonly Stack<ViewModelBase> _hierarchy = new();

        private ViewModelBase _currentViewModel;
        private ViewModelBase _rootViewModel;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void SetNavigationControl(Frame navigationControl)
        {
            _navigationControl = navigationControl;
        }
        public void Navigate<T>() where T : ViewModelBase
        {
            _currentViewModel = _serviceProvider.GetRequiredService<T>();

            _navigationControl.Navigate(_currentViewModel);
        }
        public void Navigate<T>(Action<T> action) where T : ViewModelBase
        {
            var item = _serviceProvider.GetRequiredService<T>();

            action(item);

            _currentViewModel = item;

            _navigationControl.Navigate(_currentViewModel);
        }
        public async Task NavigateAsync<T>(Func<T, Task> action) where T : ViewModelBase
        {
            var item = _serviceProvider.GetRequiredService<T>();

            await action.Invoke(item);

            _currentViewModel = item;

            _navigationControl.Navigate(_currentViewModel);
        }

        public Task NavigateAsync<T>() where T : ViewModelBase
        {
            _currentViewModel = _serviceProvider.GetRequiredService<T>();

            _navigationControl.Navigate(_currentViewModel);

            return Task.CompletedTask;
        }


        public void NavigateWithHierarchy<T>() where T : ViewModelBase
        {
            var item = _serviceProvider.GetRequiredService<T>();

            _hierarchy.Push(_currentViewModel);

            _currentViewModel = item;

            _navigationControl.Navigate(_currentViewModel);
        }

        public void NavigateWithHierarchy<T>(Action<T> action) where T : ViewModelBase
        {
            var item = _serviceProvider.GetRequiredService<T>();

            action(item);

            _hierarchy.Push(_currentViewModel);

            _currentViewModel = item;

            _navigationControl.Navigate(_currentViewModel);
        }

        public async Task NavigateWithHierarchyAsync<T>(Func<T, Task> action) where T : ViewModelBase
        {
            var item = _serviceProvider.GetRequiredService<T>();

            await action.Invoke(item);

            _hierarchy.Push(_currentViewModel);

            _currentViewModel = item;

            _navigationControl.Navigate(_currentViewModel);
        }
        public Task NavigateWithHierarchyAsync<T>() where T : ViewModelBase
        {
            var item = _serviceProvider.GetRequiredService<T>();

            _hierarchy.Push(_currentViewModel);

            _currentViewModel = item;

            _navigationControl.Navigate(_currentViewModel);

            return Task.CompletedTask;
        }

        public void NavigateBack()
        {
            _currentViewModel = _hierarchy.Pop();

            _navigationControl.Navigate(_currentViewModel);
        }
        public Task NavigateBackAsync()
        {
            _currentViewModel = _hierarchy.Pop();

            _navigationControl.Navigate(_currentViewModel);

            return Task.CompletedTask;
        }

        public void SetNavigationRoot<T>() where T : ViewModelBase
        {
            _rootViewModel = _serviceProvider.GetRequiredService<T>();
        }

        public void ResetNavigation()
        {
            _hierarchy.Clear();

            _hierarchy.Push(_rootViewModel);
        }
    }
}
