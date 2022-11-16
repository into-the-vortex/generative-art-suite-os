using System;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Extensions;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Base
{
    public class DependencyVM : IViewModel<Dependency>
    {
        public DependencyVM(Dependency model, Action<string> delete)
        {
            Model = model;
            Delete = new DelegateCommand(() => delete(model.Name));
        }

        public string Name => Model.Name;

        public ICommand Delete { get; }

        public Dependency Model { get; }

        public override string ToString()
        {
            var result = $"🔗 {Name}";

            var implicitDependencies = Model.GetDependencies().Select(m => m.Name);
            if(implicitDependencies.Any())
            {
                result += $" ({string.Join(", ", implicitDependencies)})";
            }

            return result;
        }
    }
}
