using System;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Edit
{
    public class EditLayerDialogVM : LayerDialogVM
    {
        private LayerStagingArea? layerStagingArea;
        private IDisposable? modelListener;
        private bool drawn;

        public override string Title => Strings.EditLayer;

        public override string Name
        {
            get => layerStagingArea?.Name.Value ?? string.Empty;
            set
            {
                if (layerStagingArea != null)
                {
                    layerStagingArea.Name.Value = value;
                    RaisePropertyChanged();
                }
            }
        }

        public override bool IncludeInDNA
        {
            get => layerStagingArea?.IncludeInDNA.Value ?? false;
            set
            {
                if (layerStagingArea != null)
                {
                    layerStagingArea.IncludeInDNA.Value = value;
                    RaisePropertyChanged();
                }
            }
        }

        public override bool Drawn
        {
            get => drawn;
            set => SetProperty(ref drawn, value);
        }

        public bool Optional
        {
            get => layerStagingArea is DrawnLayerStagingArea ds && ds.Optional.Value;
            set
            {
                if (layerStagingArea is DrawnLayerStagingArea ds)
                {
                    ds.Optional.Value = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool AffectedByLayerMask
        {
            get => layerStagingArea is DrawnLayerStagingArea ds && ds.AffectedByLayerMask.Value;
            set
            {
                if (layerStagingArea is DrawnLayerStagingArea ds)
                {
                    ds.AffectedByLayerMask.Value = value;
                    RaisePropertyChanged();
                }
            }
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(LayerDialogParameters.LayerStagingArea, out LayerStagingArea layerStagingArea))
            {
                this.layerStagingArea = layerStagingArea;
                modelListener = Dependencies.ConnectModelCollection(layerStagingArea.Dependencies, m => CreateDependencyVM(m));

                Drawn = layerStagingArea is DrawnLayerStagingArea;

                RaisePropertyChanged(string.Empty);
            }

            base.OnDialogOpened(parameters);
        }

        public override void OnDialogClosed()
        {
            base.OnDialogClosed();
            modelListener?.Dispose();
        }

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            var result = base.GetDialogParameters(parameter);

            if (parameter == OKAY)
            {
                result.Add(LayerDialogParameters.EditedLayer, layerStagingArea?.Apply() ?? throw new InvalidOperationException());
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
