using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.LayerDialogVM
{
    public class DeleteLayerDialogVM : DialogVM
    {
        public const string Index = nameof(Index);

        private int index;
        private Layer? layer;
        private string message = string.Empty;

        public DeleteLayerDialogVM()
        {
            Yes = new DelegateCommand(() => CloseDialog(OKAY));
            No = new DelegateCommand(() => CloseDialog(CANCEL));
        }

        public override string Title => Strings.DeleteLayer;

        public ICommand Yes { get; }

        public ICommand No { get; }

        public string Message
        {
            get => message;
            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(nameof(Message), out string message))
            {
                Message = message;
            }

            if (parameters.TryGetValue(Index, out int index))
            {
                this.index = index;
            }

            if (parameters.TryGetValue(nameof(Layer), out Layer layer))
            {
                this.layer = layer;
            }
        }

        protected override ButtonResult GetButtonResult(string parameter)
        {
            switch (parameter)
            {
                case OKAY:
                    return ButtonResult.OK;
                case CANCEL:
                    return ButtonResult.Cancel;
                default:
                    return ButtonResult.None;
            }
        }

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            if (layer is null)
            {
                throw new InvalidOperationException();
            }

            return new DialogParameters()
            {
                { Index, index },
                { nameof(Layer), layer },
            };
        }
    }
}
