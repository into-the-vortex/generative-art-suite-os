using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public class EditLayerDialogVM : LayerDialogVM
    {
        public const string OldLayer = nameof(OldLayer);
        public const string NewLayer = nameof(NewLayer);

        private Layer? oldLayer;

        public override string Title => Strings.EditLayer;

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(nameof(Layer), out Layer layer))
            {
                oldLayer = layer;
                Name = layer.Name;
                Optional = layer.Optional;
                IncludeInDNA = layer.IncludeInDNA;
                AffectedByLayerMask = layer.AffectedByLayerMask;
                UpdatePathSource(layer.Paths);
            }
        }

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            var result = new DialogParameters();

            if (parameter == OKAY && oldLayer != null)
            {
                result.Add(NewLayer, Create());
                result.Add(OldLayer, oldLayer);
            }

            return result;
        }
    }
}
