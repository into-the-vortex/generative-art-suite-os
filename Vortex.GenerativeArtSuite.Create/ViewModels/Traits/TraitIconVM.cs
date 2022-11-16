using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitIconVM : BindableBase, IViewModel<Trait>
    {
        public TraitIconVM(Trait model, Action<Trait> editCallback, Action<Trait> deleteCallback)
        {
            Model = model;
            Edit = new DelegateCommand(() => editCallback(model), () => IsEditable);
            Delete = new DelegateCommand(() => deleteCallback(model), () => IsEditable);
        }

        public string Name => Model.Name;

        public string IconURI => Model.IconURI;

        public string WeightLabel => $"{Strings.Weight} - {Weight:D3}";

        public bool IsEditable => Model is not NoneTrait;

        public int Weight
        {
            get => Model.Weight;
            set
            {
                if (Model.Weight != value)
                {
                    Model.Weight = value;
                    RaisePropertyChanged(nameof(Weight));
                    RaisePropertyChanged(nameof(WeightLabel));
                }
            }
        }

        public ICommand Edit { get; }

        public ICommand Delete { get; }

        public Trait Model { get; }
    }
}
