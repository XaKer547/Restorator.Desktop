using Restorator.Domain.Models.Restaurant;
using System.Windows;

namespace Restorator.Desktop.Infrastructure
{
    public class RestaurantPreviewBindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new RestaurantPreviewBindingProxy();
        }

        public RestaurantPreviewDTO Data
        {
            get
            {
                return (RestaurantPreviewDTO)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(RestaurantPreviewDTO), typeof(RestaurantPreviewBindingProxy), new UIPropertyMetadata(null));
    }
}
