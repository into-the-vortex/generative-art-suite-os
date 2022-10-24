using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitsVM : IViewModel<Layer>
    {
        public TraitsVM(Layer layer)
        {
            Model = layer;
            Add = new DelegateCommand(OnAdd);
            TraitVms.ConnectModelCollection(layer.Traits, (t) => new TraitVM(t));
        }

        public ICommand Add { get; }

        public ObservableCollection<TraitVM> TraitVms { get; } = new();

        public Layer Model { get; }

        private void OnAdd()
        {
        }
    }
}
