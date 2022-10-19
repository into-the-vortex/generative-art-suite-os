using System.Collections.Generic;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public class CreateLayerDialogVM : LayerDialogVM
    {
        private int index;
        private int maxIndex;

        public override string Title => Strings.CreateLayer;

        public int Index
        {
            get => index;
            set
            {
                if (index != value && value >= 0 && value <= maxIndex)
                {
                    index = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxIndex
        {
            get => maxIndex;
            set
            {
                if (maxIndex != value)
                {
                    maxIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(ExistingLayerNames, out List<string> layerNames))
            {
                MaxIndex = layerNames.Count;
                Index = MaxIndex;
            }
        }

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            var result = new DialogParameters();

            if (parameter == OKAY)
            {
                result.Add(nameof(Index), Index);
                result.Add(nameof(Layer), Create());
            }

            return result;
        }

        protected override bool CanConfirm()
        {
            return base.CanConfirm() &&
                Index >= 0 &&
                Index <= MaxIndex;
        }
    }
}
