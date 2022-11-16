using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class DependencyTraitVM : TraitVM
    {
        public DependencyTraitVM(IFileSystem fileSystem, DependencyTraitStagingArea traitStagingArea, Action raiseCanExecuteChanged)
            : base(fileSystem, traitStagingArea, raiseCanExecuteChanged)
        {
            VariantVMs.ConnectModelCollection(traitStagingArea.Variants, m => new TraitVariantVM(fileSystem, m, raiseCanExecuteChanged));
        }

        public ObservableCollection<TraitVariantVM> VariantVMs { get; } = new();

        public override bool CanConfirm()
        {
            return VariantVMs.All(v => File.Exists(v.Trait.URI)) &&
                base.CanConfirm();
        }
    }
}
