using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Models.Generating
{
    public static class ImageBuilder
    {
        private static readonly Dictionary<string, Image> Images = new();

        private static CancellationTokenSource cancellationTokenSource = new();
        private static Task? cacheBuild;

        public static void BuildCache(Session session)
        {
            cancellationTokenSource.Cancel();
            WaitForCacheBuild();

            cancellationTokenSource = new();
            cacheBuild = Task.Run(() =>
            {
                try
                {
                    foreach (var image in Images)
                    {
                        image.Value.Dispose();
                    }

                    Images.Clear();

                    var images = session.GetTraitURIs()
                        .Concat(session.GetMaskURIs())
                        .Where(uri => File.Exists(uri));

                    foreach(var image in images)
                    {
                        Images[image] = Image.FromFile(image);
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

        public static Bitmap Build(IRespectCheckpoint checkpoint, List<GenerationStep> steps)
        {
            WaitForCacheBuild();

            // TODO: Masking
            var images = steps.OfType<DrawnGenerationStep>().Select(s => Images[s.ImageURI]);

            var widths = new List<int>();
            var heights = new List<int>();
            foreach (var image in images)
            {
                lock (image)
                {
                    widths.Add(image.Width);
                    heights.Add(image.Height);
                }
            }

            var width = widths.Max();
            var height = heights.Max();

            var canvas = new Bitmap(width, height);

            using (var g = Graphics.FromImage(canvas))
            {
                foreach (var image in images)
                {
                    checkpoint.RespectCheckpoint();

                    lock (image)
                    {
                        g.DrawImage(image, new Rectangle(0, 0, width, height));
                    }
                }
            }

            return canvas;
        }

        private static void WaitForCacheBuild()
        {
            cacheBuild?.Wait();
            cacheBuild?.Dispose();
            cacheBuild = null;
        }
    }
}
