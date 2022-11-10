using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Base
{
    public abstract class YesNoDialogVM : DialogVM
    {
        private string message = string.Empty;

        public YesNoDialogVM()
        {
            Yes = new DelegateCommand(() => CloseDialog(OKAY));
            No = new DelegateCommand(() => CloseDialog(CANCEL));
        }

        public ICommand Yes { get; }

        public ICommand No { get; }

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(nameof(Message), out string message))
            {
                Message = message;
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
    }
}
