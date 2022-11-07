using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public static class ImageBuilder
    {
        public static Task<Bitmap> Build(List<GenerationStep> steps, CancellationToken cancellationToken)
        {
            // TODO: Masking
            return Task.Run(
                () =>
            {
                var images = steps.Select(s => Image.FromFile(s.ImageURI));
                var width = images.Max(i => i.Width);
                var height = images.Max(i => i.Height);

                var canvas = new Bitmap(width, height);
                using (var g = Graphics.FromImage(canvas))
                {
                    foreach (var image in images)
                    {
                        if(cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        g.DrawImage(image, new Rectangle(0, 0, width, height));
                    }
                }

                return canvas;
            }, cancellationToken);
        }
    }
}
