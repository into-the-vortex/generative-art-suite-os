﻿using System;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Base
{
    public abstract class DialogVM : BindableBase, IDialogAware
    {
        public const string CreateLayerDialog = nameof(CreateLayerDialog);
        public const string EditLayerDialog = nameof(EditLayerDialog);
        public const string DeleteLayerDialog = nameof(DeleteLayerDialog);

        public const string CreateTraitDialog = nameof(CreateTraitDialog);
        public const string EditTraitDialog = nameof(EditTraitDialog);
        public const string DeleteTraitDialog = nameof(DeleteTraitDialog);

        public const string ConfirmSaveDialog = nameof(ConfirmSaveDialog);

        public const string OKAY = "ok";
        public const string CANCEL = "cancel";

        public event Action<IDialogResult>? RequestClose;

        public abstract string Title { get; }

        public abstract void OnDialogOpened(IDialogParameters parameters);

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        protected void CloseDialog(string parameter)
        {
            RaiseRequestClose(new DialogResult(GetButtonResult(parameter), GetDialogParameters(parameter)));
        }

        protected void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        protected virtual IDialogParameters GetDialogParameters(string parameter)
        {
            return new DialogParameters();
        }

        protected abstract ButtonResult GetButtonResult(string parameter);
    }
}
