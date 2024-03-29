﻿using System;
using System.IO;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class DrawnTraitVM : TraitVM, ITraitVariantVM
    {
        public DrawnTraitVM(IFileSystem fileSystem, DrawnTraitStagingArea traitStagingArea, Action raiseCanExecuteChanged)
            : base(fileSystem, traitStagingArea, raiseCanExecuteChanged)
        {
            Trait = new TraitImageVM(
                fileSystem,
                Strings.AddTrait,
                () => traitStagingArea.TraitURI.Value,
                val => traitStagingArea.TraitURI.Value = val,
                raiseCanExecuteChanged);

            Mask = new TraitImageVM(
                fileSystem,
                Strings.AddMask,
                () => traitStagingArea.MaskURI.Value,
                val => traitStagingArea.MaskURI.Value = val,
                raiseCanExecuteChanged);
        }

        public TraitImageVM Trait { get; }

        public TraitImageVM Mask { get; }

        public override bool CanConfirm()
        {
            return File.Exists(Trait.URI) &&
                base.CanConfirm();
        }
    }
}
