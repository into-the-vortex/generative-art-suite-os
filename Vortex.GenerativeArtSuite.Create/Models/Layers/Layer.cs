using System.Collections.Generic;
using DynamicData;
using Vortex.GenerativeArtSuite.Create.Extensions;
using Vortex.GenerativeArtSuite.Create.Models.Settings;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.Models.Layers
{
    public abstract class Layer : IDependencyOwner
    {
        protected Layer()
            : this(string.Empty, true, new())
        {
        }

        protected Layer(string name, bool includeInDNA, List<Dependency> dependencies)
        {
            Name = name;
            IncludeInDNA = includeInDNA;
            Dependencies = dependencies;
            Traits = new List<Trait>();
        }

        public string Name { get; set; }

        public bool IncludeInDNA { get; set; }

        public List<Dependency> Dependencies { get; }

        public List<Trait> Traits { get; }

        public Trait SelectRandomTrait()
        {
            return Traits.SelectRandom();
        }

        public bool EditDependency(Dependency oldDep, Dependency newDep)
        {
            bool result = false;

            void EditInChildren(IDependencyOwner child)
            {
                foreach (var dep in child.Dependencies)
                {
                    EditInChildren(dep);

                    if (dep.Dependencies.Contains(oldDep))
                    {
                        dep.Dependencies.Replace(oldDep, newDep);
                        result |= true;
                    }
                }
            }

            EditInChildren(this);

            if (Dependencies.Contains(oldDep))
            {
                Dependencies.Replace(oldDep, newDep);
                result |= true;
            }

            return result;
        }

        public bool RemoveDependency(Dependency dependency)
        {
            bool result = false;

            void RemoveFromChildren(IDependencyOwner child)
            {
                foreach (var dep in child.Dependencies)
                {
                    RemoveFromChildren(dep);
                    result |= dep.Dependencies.Remove(dependency);
                }
            }

            RemoveFromChildren(this);

            result |= Dependencies.Remove(dependency);

            return result;
        }

        public abstract Trait CreateTrait(string name = "", string iconURI = "");

        public abstract void EnsureTraitsRemainValid(Session session);
    }
}
