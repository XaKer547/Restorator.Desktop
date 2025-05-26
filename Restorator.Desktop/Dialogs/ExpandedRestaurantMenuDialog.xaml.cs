using Wpf.Ui.Controls;

namespace Restorator.Desktop.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ExpandedRestaurantMenuDialog.xaml
    /// </summary>
    public partial class ExpandedRestaurantMenuDialog : ContentDialog
    {
        public string Image { get; set; }
        public ExpandedRestaurantMenuDialog(string image)
        {
            Image = image;
            DataContext = this;
            InitializeComponent();
        }
    }
}
