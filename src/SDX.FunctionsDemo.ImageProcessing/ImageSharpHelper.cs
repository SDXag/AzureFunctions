using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SDX.FunctionsDemo.ImageProcessing
{
    internal static class ImageSharpHelper
    {
        public static byte[] Resize(byte[] data, int size)
        {
            return Process(data, x => x.Resize(new ResizeOptions { Size = new Size(size, size), Mode = ResizeMode.Pad }));
        }

        public static byte[] GrayScale(byte[] data, int size)
        {
            return Process(data, x => x.Resize(size, size).Grayscale());
        }

        public static byte[] Sepia(byte[] data, int size)
        {
            return Process(data, x => x.Resize(size, size).Sepia());
        }

        public static byte[] OilPaint(byte[] data, int size)
        {
            return Process(data, x => x.Resize(size, size).OilPaint());
        }

        public static byte[] GaussianBlur(byte[] data, int size)
        {
            return Process(data, x => x.Resize(size, size).GaussianBlur());
        }

        public static byte[] RoundImage(byte[] data, int size)
        {
            return Process(data, x => x.Resize(size, size).ApplyRoundedCorners(size));
        }

        private static byte[] Process(byte[] data, Action<IImageProcessingContext> operation)
        {
            using (var img = Image.Load<Rgba32>(data, new PngDecoder()))
            {
                img.Mutate(operation);
                var ms = new MemoryStream();
                var encoder = (PngEncoder)img.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
                img.SaveAsPng(ms, encoder);
                return ms.ToArray();
            }
        }

        private static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, int size)
        {
            ctx.SetGraphicsOptions(new GraphicsOptions()
            {
                Antialias = true,
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut // enforces that any part of this shape that has color is punched out of the background
            });

            var corners = BuildCorners(size);
            foreach (var c in corners)
                ctx = ctx.Fill(Color.Red, c);

            return ctx;
        }

        private static IPathCollection BuildCorners(int size)
        {
            float cornerRadius = size / 2;

            // first create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            var cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the original around the center of the image

            var rightPos = size - cornerTopLeft.Bounds.Width + 1;
            var bottomPos = size - cornerTopLeft.Bounds.Height + 1;

            // move it across the width of the image - the width of the shape
            var cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            var cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            var cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }
}
