using System;
using System.Linq;

namespace SDX.FunctionsDemo.ImageProcessing
{
    public static class ImageTypes
    {
        public static readonly ImageType Original = new ImageType { Size = -1 };

        public static readonly ImageType[] Catalogue = new ImageType[]
        {
            new ImageType { Size=100 },
            new ImageType { Size=200 },
            new ImageType { Size=200, Effect = Effect.GrayScale },
            new ImageType { Size=200, Effect = Effect.Sepia },
            new ImageType { Size=200, Effect = Effect.OilPaint },
            new ImageType { Size=200, Effect = Effect.GaussianBlur },
            new ImageType { Size=200, Effect = Effect.RoundImage },
            new ImageType { Size=400 },
            new ImageType { Size=500 },
         };
    }
}
