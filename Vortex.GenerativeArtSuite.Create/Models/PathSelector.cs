using System;
using System.Collections.Generic;
using System.Linq;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class PathSelector
    {
        public PathSelector(string path)
        {
            Options = new(path.Split("/").Select(o => o.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
            Path = string.Join(" / ", Options);
        }

        public string Path { get; }

        public List<string> Options { get; }

        public string Follow(IEnumerable<string> existingPath)
        {
            throw new NotImplementedException();
        }
    }
}
