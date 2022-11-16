using System;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Models.Layers;
using Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Create
{
    public class CreateLayerDialogVM : LayerDialogVM
    {
        private IDisposable modelListener;

        private Layer model = new DrawnLayer();

        private bool affectedByLayerMask = new DrawnLayer().AffectedByLayerMask;
        private bool optional = new DrawnLayer().Optional;
        private bool drawn = true;

        public CreateLayerDialogVM()
        {
            modelListener = Dependencies.ConnectModelCollection(model.Dependencies, m => CreateDependencyVM(m));
        }

        public override string Title => Strings.CreateLayer;

        public override string Name
        {
            get => model.Name;
            set
            {
                if (model.Name != value)
                {
                    model.Name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public override bool IncludeInDNA
        {
            get => model.IncludeInDNA;
            set
            {
                if (model.IncludeInDNA != value)
                {
                    model.IncludeInDNA = value;
                    RaisePropertyChanged();
                }
            }
        }

        public override bool Drawn
        {
            get => drawn;
            set => SetProperty(ref drawn, value, OnDrawnChanged);
        }

        public bool Optional
        {
            get => optional;
            set
            {
                if (model is DrawnLayer dl)
                {
                    dl.Optional = value;
                    dl.EnsureOptional();
                }

                optional = value;
                RaisePropertyChanged();
            }
        }

        public bool AffectedByLayerMask
        {
            get => affectedByLayerMask;
            set
            {
                if (model is DrawnLayer dl)
                {
                    dl.AffectedByLayerMask = value;
                }

                affectedByLayerMask = value;
                RaisePropertyChanged();
            }
        }

        public override void OnDialogClosed()
        {
            base.OnDialogClosed();
            modelListener.Dispose();
        }

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            var result = base.GetDialogParameters(parameter);

            if (parameter == OKAY)
            {
                result.Add(LayerDialogParameters.CreatedLayer, model);
            }

            return result;
        }

        private void OnDrawnChanged()
        {
            modelListener.Dispose();

            if (drawn)
            {
                model = new DrawnLayer(model.Name, model.IncludeInDNA, model.Dependencies, optional, affectedByLayerMask);
            }
            else
            {
                model = new PathingLayer(model.Name, model.IncludeInDNA, model.Dependencies);
            }

            modelListener = Dependencies.ConnectModelCollection(model.Dependencies, m => CreateDependencyVM(m));
        }
    }
}
