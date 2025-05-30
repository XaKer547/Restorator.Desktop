using CommunityToolkit.Mvvm.ComponentModel;

namespace Restorator.Desktop.Models
{
    public partial class RestaurantImageDTO : ObservableObject
    {
        [ObservableProperty]
        private string source;

        [ObservableProperty]
        private bool isLocal = false;
    }
}
