using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Settings
{
    public class GenerationSettingsVM : BindableBase, IViewModel<GenerationSettings>
    {
        public GenerationSettingsVM(GenerationSettings settings)
        {
            Model = settings;
        }

        public string NamePrefix
        {
            get => Model.NamePrefix;
            set
            {
                if (Model.NamePrefix != value)
                {
                    Model.NamePrefix = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string DescriptionTemplate
        {
            get => Model.DescriptionTemplate;
            set
            {
                if (Model.DescriptionTemplate != value)
                {
                    Model.DescriptionTemplate = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string BaseURI
        {
            get => Model.BaseURI;
            set
            {
                if (Model.BaseURI != value)
                {
                    Model.BaseURI = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ExternalUrl
        {
            get => Model.ExternalUrl;
            set
            {
                if (Model.ExternalUrl != value)
                {
                    Model.ExternalUrl = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int CollectionSize
        {
            get => Model.CollectionSize;
            set
            {
                if (Model.CollectionSize != value)
                {
                    Model.CollectionSize = value;
                    RaisePropertyChanged();
                }
            }
        }

        public GenerationSettings Model { get; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(NamePrefix) &&
                !string.IsNullOrWhiteSpace(DescriptionTemplate) &&
                !string.IsNullOrWhiteSpace(BaseURI) &&
                CollectionSize > 0;
        }
    }
}
