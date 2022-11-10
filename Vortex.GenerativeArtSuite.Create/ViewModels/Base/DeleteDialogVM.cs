using Prism.Services.Dialogs;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Base
{
    public abstract class DeleteDialogVM : YesNoDialogVM
    {
        public const string Index = nameof(Index);

        private int index;

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);

            if (parameters.TryGetValue(Index, out int index))
            {
                this.index = index;
            }
        }

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            return new DialogParameters()
            {
                { Index, index },
            };
        }
    }
}
