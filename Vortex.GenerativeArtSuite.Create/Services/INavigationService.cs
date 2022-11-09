using Prism.Regions;
using System.ComponentModel;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface INavigationService : INotifyPropertyChanged
    {
        string? CurrentView { get; set; }

        void NavigateTo(string? tag, NavigationParameters? parameters = null);
    }
}
