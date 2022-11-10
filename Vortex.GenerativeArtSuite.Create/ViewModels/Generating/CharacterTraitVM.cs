using System;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generating
{
    public class CharacterTraitVM
    {
        public CharacterTraitVM(Trait model, Action<Trait> onClick)
        {
            Name = model.Name;
            Icon = model.IconURI;
            OnClick = new DelegateCommand(() => onClick(model));
        }

        public string Name { get; }

        public string? Icon { get; }

        public ICommand OnClick { get; }
    }
}
