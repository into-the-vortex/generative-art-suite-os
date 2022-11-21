using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Home
{
    public class ConfirmSaveDialogVM : DialogVM
    {
        private readonly ISessionManager sessionManager;
        private string message = string.Empty;
        private bool busy;

        public ConfirmSaveDialogVM(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;

            Save = new DelegateCommand(OnClickSave, CanClickButton).ObservesProperty(() => Busy);
            Exit = new DelegateCommand(() => CloseDialog(CANCEL), CanClickButton).ObservesProperty(() => Busy);
        }

        public override string Title => Strings.SaveSession;

        public ICommand Save { get; }

        public ICommand Exit { get; }

        public string SaveCTA => busy ? Strings.Syncing : Strings.ExitSave;

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public bool Busy
        {
            get => busy;
            set => SetProperty(ref busy, value, () => RaisePropertyChanged(nameof(SaveCTA)));
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
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

        private void OnClickSave()
        {
            Task.Run(async () =>
            {
                try
                {
                    Busy = true;

                    await sessionManager.SaveSession(message);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CloseDialog(OKAY);
                    });
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    Busy = false;
                }
            });
        }

        private bool CanClickButton() => !busy;
    }
}
