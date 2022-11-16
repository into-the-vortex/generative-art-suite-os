using System;
using Vortex.GenerativeArtSuite.Common.Staging;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Staging
{
    public abstract class LayerStagingArea
    {
        private readonly Layer model;

        protected LayerStagingArea(Layer model, Action onDependencyChanged)
        {
            this.model = model;

            Name = new StagingProperty<string>(name => model.Name = name, () => model.Name, onDependencyChanged);
            IncludeInDNA = new StagingProperty<bool>(includeInDNA => model.IncludeInDNA = includeInDNA, () => model.IncludeInDNA);
            Dependencies = new StagingList<Dependency>(model.Dependencies, onDependencyChanged);
        }

        public StagingProperty<string> Name { get; }

        public StagingProperty<bool> IncludeInDNA { get; }

        public StagingList<Dependency> Dependencies { get; }

        public virtual Layer Apply()
        {
            Name.Apply();
            IncludeInDNA.Apply();
            Dependencies.Apply();

            return model;
        }

        public virtual bool IsDirty()
        {
            return Name.IsDirty ||
                IncludeInDNA.IsDirty ||
                Dependencies.IsDirty;
        }
    }
}
