using System;
using System.Diagnostics;

namespace SDX.FunctionsDemo.ImageProcessing
{
    /// <summary>Size + Effect</summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class ImageType
    {
        public int Size { get; set; }
        public Effect Effect { get; set; }

        public override string ToString()
        {
            return Size + " " + Effect;
        }

        private string GetDebuggerDisplay()
        {
            return "Size=" + Size + "; Effect=" + Effect;
        }

        public static ImageType TryParse(string size, string effect)
        {
            if (!int.TryParse(size, out var s))
                return null;
            if (!Enum.TryParse<Effect>(effect, out var e))
                return null;
            return new ImageType { Size = s, Effect = e };
        }
    }
}
