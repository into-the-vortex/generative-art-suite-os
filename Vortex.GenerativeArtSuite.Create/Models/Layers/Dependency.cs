using System;
using System.Collections.Generic;

namespace Vortex.GenerativeArtSuite.Create.Models.Layers
{
    public class Dependency : IDependencyOwner, IEquatable<Dependency?>
    {
        public Dependency()
        {
            Name = string.Empty;
            Dependencies = new List<Dependency>();
        }

        public Dependency(Layer source)
        {
            Name = source.Name;
            Dependencies = new List<Dependency>(source.Dependencies);
        }

        public string Name { get; set; }

        public List<Dependency> Dependencies { get; }

        public static bool operator ==(Dependency? left, Dependency? right)
        {
            return EqualityComparer<Dependency>.Default.Equals(left, right);
        }

        public static bool operator !=(Dependency? left, Dependency? right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Dependency);
        }

        public bool Equals(Dependency? other)
        {
            return other is not null &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
