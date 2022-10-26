using System.Collections.Generic;
using Prism.Services.Dialogs;

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
            set => SetProperty(ref index, value >= 0 && value <= maxIndex ? value : index);
        }

        public int MaxIndex
        {
            get => maxIndex;
            set => SetProperty(ref maxIndex, value);
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
            var result = base.GetDialogParameters(parameter);

            if (parameter == OKAY)
            {
                result.Add(nameof(Index), index);
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
