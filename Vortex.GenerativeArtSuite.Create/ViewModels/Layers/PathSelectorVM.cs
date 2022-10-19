using System;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public class PathSelectorVM : IViewModel<PathSelector>
    {
        public PathSelectorVM(PathSelector model, Action<PathSelectorVM> onDelete)
        {
            Delete = new DelegateCommand(() => onDelete(this));
            Model = model;
        }

        public string Path => Model.Path;

        public ICommand Delete { get; }

        public PathSelector Model { get; }
    }
}
