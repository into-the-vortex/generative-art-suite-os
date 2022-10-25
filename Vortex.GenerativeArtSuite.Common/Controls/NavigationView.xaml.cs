using System.Linq;
using System.Windows;
using ModernWpf.Controls;

namespace Vortex.GenerativeArtSuite.Common.Controls
{
    public partial class NavigationView
    {
        public static readonly DependencyProperty SelectedTagProperty =
            DependencyProperty.Register("SelectedTag", typeof(string), typeof(NavigationView), new PropertyMetadata(default));

        public NavigationView()
        {
            InitializeComponent();
        }

        public string SelectedTag
        {
            get => (string)GetValue(SelectedTagProperty);
            set => SetValue(SelectedTagProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SelectedItemProperty &&
                e.NewValue is NavigationViewItem { Tag: string tag })
            {
                SelectedTag = tag;
            }

            if (e.Property == SelectedTagProperty &&
                e.NewValue is string newTag &&
                MenuItemsSource is NavigationViewItem[] items)
            {
                SelectedItem = items.FirstOrDefault(item => item.Tag is string other && other == newTag);
            }

            if (e.Property == MenuItemsSourceProperty &&
                MenuItemsSource is NavigationViewItem[] menuItems)
            {
                SelectedItem = menuItems.FirstOrDefault(item => item.Tag is string other && other == SelectedTag);
            }
        }
    }
}
