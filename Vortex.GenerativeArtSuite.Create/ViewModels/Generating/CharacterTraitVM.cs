using System;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generating
{
    public class CharacterTraitVM
    {
        public CharacterTraitVM(Trait model, bool selected, Action<Trait> onClick)
        {
            Name = model.Name;
            Icon = model.IconURI;
            OnClick = new DelegateCommand(() => onClick(model), () => !selected);
            Selected = selected;
        }

        public string Name { get; }

        public string Icon { get; }

        public bool Selected { get; }

        public ICommand OnClick { get; }
    }
}
