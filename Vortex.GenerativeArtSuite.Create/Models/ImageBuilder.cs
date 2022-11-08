using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public static class ImageBuilder
    {
        public static Bitmap Build(IGenerationProcess process, List<GenerationStep> steps)
        {
            // TODO: Masking
            var images = steps.Select(s => Image.FromFile(s.ImageURI));
            var width = images.Max(i => i.Width);
            var height = images.Max(i => i.Height);

            var canvas = new Bitmap(width, height);
            using (var g = Graphics.FromImage(canvas))
            {
                foreach (var image in images)
                {
                    process.RespectCheckpoint();
                    g.DrawImage(image, new Rectangle(0, 0, width, height));
                }
            }

            return canvas;
        }
    }
}
