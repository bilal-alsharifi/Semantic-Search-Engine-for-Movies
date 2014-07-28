using System;
using System.Drawing;
using VideoTrack.Geometry;

namespace VideoTrack
{
    public static class LayoutFactory
    {
        public static ILayout CrateLayout(LayoutType layoutType, SizeF size)
        {
            switch (layoutType)
            {
                case LayoutType.Typewriter:
                    return new TypewriterLayout(size);

                case LayoutType.Spiral:
                    return new SpiralLayout(size);
            
                default:
                    throw new ArgumentException(string.Format("No constructor specified to create a layout instance for {0}.", layoutType), "layoutType");
            }
        }
    }
}
