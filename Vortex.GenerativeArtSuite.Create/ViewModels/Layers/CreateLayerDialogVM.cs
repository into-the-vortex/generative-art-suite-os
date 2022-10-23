using System.Collections.Generic;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public class CreateLayerDialogVM : LayerDialogVM
    {
        private readonly List<PathSelector> paths = new();
        private string name = string.Empty;
        private bool optional;
        private bool includeInDNA = true;
        private bool affectedByLayerMask = true;
        private int index;
        private int maxIndex;

        public CreateLayerDialogVM()
        {
            PathVMs.ConnectModelCollection(paths, m => new PathSelectorVM(m, RemovePath));
        }

        public override string Title => Strings.CreateLayer;

        public override string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        public override bool Optional
        {
            get => optional;
            set
            {
                if (optional != value)
                {
                    optional = value;
                    OnPropertyChanged();
                }
            }
        }

        public override bool IncludeInDNA
        {
            get => includeInDNA;
            set
            {
                if (includeInDNA != value)
                {
                    includeInDNA = value;
                    OnPropertyChanged();
                }
            }
        }

        public override bool AffectedByLayerMask
        {
            get => affectedByLayerMask;
            set
            {
                if (affectedByLayerMask != value)
                {
                    affectedByLayerMask = value;
                    OnPropertyChanged();
                }
            }
        }

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
                result.Add(nameof(Index), index);
                result.Add(nameof(Layer), new Layer(name, optional, includeInDNA, affectedByLayerMask, paths));
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
