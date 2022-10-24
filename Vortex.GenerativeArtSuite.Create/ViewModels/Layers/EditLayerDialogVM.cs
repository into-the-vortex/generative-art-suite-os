using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Staging;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public class EditLayerDialogVM : LayerDialogVM
    {
        private LayerStagingArea? layerStagingArea;

        public override string Title => Strings.EditLayer;

        public override string Name
        {
            get => layerStagingArea == null ? string.Empty : layerStagingArea.Name.Value;
            set
            {
                if (layerStagingArea != null)
                {
                    layerStagingArea.Name.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public override bool Optional
        {
            get => layerStagingArea != null && layerStagingArea.Optional.Value;
            set
            {
                if (layerStagingArea != null)
                {
                    layerStagingArea.Optional.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public override bool IncludeInDNA
        {
            get => layerStagingArea != null && layerStagingArea.IncludeInDNA.Value;
            set
            {
                if (layerStagingArea != null)
                {
                    layerStagingArea.IncludeInDNA.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public override bool AffectedByLayerMask
        {
            get => layerStagingArea != null && layerStagingArea.AffectedByLayerMask.Value;
            set
            {
                if (layerStagingArea != null)
                {
                    layerStagingArea.AffectedByLayerMask.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(nameof(LayerStagingArea), out LayerStagingArea layerStagingArea))
            {
                this.layerStagingArea = layerStagingArea;
                PathVMs.ConnectModelCollection(layerStagingArea.Paths, m => new PathSelectorVM(m, RemovePath));

                OnPropertyChanged(string.Empty);
            }
        }

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            var result = new DialogParameters();

            if (parameter == OKAY)
            {
                layerStagingArea?.Apply();

                result.Add(nameof(Name), Name);
            }

            return result;
        }

        protected override bool CanConfirm()
        {
            return base.CanConfirm() &&
                layerStagingArea?.IsDirty() == true;
        }
    }
}
