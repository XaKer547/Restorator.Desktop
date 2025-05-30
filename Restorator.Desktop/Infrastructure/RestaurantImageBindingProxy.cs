using Restorator.Desktop.Models;
using System.Windows;

namespace Restorator.Desktop.Infrastructure
{
    public class RestaurantImageBindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new RestaurantImageBindingProxy();
        }

        public RestaurantImageDTO Data
        {
            get
            {
                return (RestaurantImageDTO)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(RestaurantImageDTO), typeof(RestaurantImageBindingProxy), new UIPropertyMetadata(null));
    }
}
