using System;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class RecentSessionVM
    {
        private RecentSession model;

        public RecentSessionVM(RecentSession model, Action<string> onOpen)
        {
            this.model = model;
            OnClick = new DelegateCommand(() => onOpen(model.Name));
        }

        public string Name => model.Name;

        public string Created => $"{Strings.Created} {model.Created.ToShortTimeString()} {model.Created.ToShortDateString()}";

        public string Modified => $"{Strings.Modified} {model.Modified.ToShortTimeString()} {model.Modified.ToShortDateString()}";

        public ICommand OnClick { get; set; }
    }
}
