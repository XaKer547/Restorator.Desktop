using CommunityToolkit.Mvvm.ComponentModel;
using Restorator.Domain.Models.Enums;

namespace Restorator.Desktop.ViewModels.Abstract
{
    public partial class AuthenticationViewModelBase : ViewModelBase
    {
        [ObservableProperty]
        private Roles role;
    }
}