using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class LayerVM
    {
        private readonly Layer model;

        public LayerVM(Layer model)
        {
            this.model = model;
        }

        public string Name => model.Name;
    }
}
