using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class LayerTraitVM : IViewModel<Layer>
    {
        private readonly bool isMultiTrait;
        private List<string> paths = new();

        public LayerTraitVM(Layer layer)
        {
            Model = layer;
            isMultiTrait = Model.Paths.Any();

            if(layer.Optional)
            {
                TraitVms.Add(new NoneTraitVM());
            }

            if (isMultiTrait)
            {
                ExtractTraitPaths();
                ExtractExistingMultiTraits();
            }
            else
            {
                ExtractExistingTraits();
            }
        }

        public ObservableCollection<object> TraitVms { get; } = new();

        public Layer Model { get; }

        public void Add()
        {
        }

        public void MultiAdd()
        {
        }

        private void ExtractTraitPaths()
        {

        }

        private void ExtractExistingTraits()
        {
            TraitVms.AddRange(Model.Traits.Select(t => new SingleTraitVM(t)));
        }

        private void ExtractExistingMultiTraits()
        {
            //var displayNames = Model.Traits.Select(t => t.DisplayName).Distinct();
            //TraitVms.AddRange(displayNames.Select(dn => new MultiTraitVM(Model.Traits.Where(t => t.DisplayName == dn), paths)));
        }
    }
}
