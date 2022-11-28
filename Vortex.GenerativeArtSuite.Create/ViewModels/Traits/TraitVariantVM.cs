using System;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models.Traits;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitVariantVM : BindableBase, IViewModel<TraitVariant>, ITraitVariantVM
    {
        public TraitVariantVM(IFileSystem fileSystem, TraitVariant model, Action raiseCanExecuteChanged, Action<TraitVariantVM> onTraitBrowseSuccess)
        {
            VariantPath = model.VariantPath;

            Trait = new TraitImageVM(
                fileSystem,
                Strings.AddTrait,
                () => model.TraitURI,
                val => model.TraitURI = val,
                raiseCanExecuteChanged,
                () => onTraitBrowseSuccess(this));

            Mask = new TraitImageVM(
                fileSystem,
                Strings.AddMask,
                () => model.MaskURI,
                val => model.MaskURI = val,
                raiseCanExecuteChanged);

            Model = model;
        }

        public string VariantPath { get; }

        public TraitImageVM Trait { get; }

        public TraitImageVM Mask { get; }

        public TraitVariant Model { get; }
    }
}
