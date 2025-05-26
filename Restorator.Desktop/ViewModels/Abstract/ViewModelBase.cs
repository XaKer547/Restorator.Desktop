using CommunityToolkit.Mvvm.ComponentModel;

namespace Restorator.Desktop.ViewModels.Abstract
{
    public abstract partial class ViewModelBase : ObservableValidator
    {
        [ObservableProperty]
        public bool initialized = false;
        protected bool CanInitialize => !Initialized;
    }
}