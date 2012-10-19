using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Windows.Forms;

namespace Graphics
{
    public class OrientationUtil
    {
        /// <summary>
        /// Places a rectangle with [size] size into area [area], with offset [offset] from the border and oriented towards [orientation]
        /// Returns the position of the rectangle
        /// </summary>
        public static Vector2 Position(Orientation orientation, Vector2 area, Vector2 offset, Vector2 size)
        {
            switch (orientation)
            {
                case Orientation.Bottom:
                    return Position(AnchorStyles.Bottom, area, offset, size);
                case Orientation.BottomLeft:
                    return Position(AnchorStyles.Bottom | AnchorStyles.Left, area, offset, size);
                case Orientation.BottomRight:
                    return Position(AnchorStyles.Bottom | AnchorStyles.Right, area, offset, size);
                case Orientation.Center:
                    return Position(AnchorStyles.None, area, offset, size);
                case Orientation.Left:
                    return Position(AnchorStyles.Left, area, offset, size);
                case Orientation.Right:
                    return Position(AnchorStyles.Right, area, offset, size);
                case Orientation.Top:
                    return Position(AnchorStyles.Top, area, offset, size);
                case Orientation.TopLeft:
                    return Position(AnchorStyles.Top | AnchorStyles.Left, area, offset, size);
                case Orientation.TopRight:
                    return Position(AnchorStyles.Top | AnchorStyles.Right, area, offset, size);
                default:
                    throw new ArgumentException();
            }
        }
        /// <summary>
        /// Places a rectangle with [size] size into area [area], with offset [offset] from the border and oriented towards [orientation]
        /// Returns the position of the rectangle
        /// </summary>
        public static Vector2 Position(AnchorStyles orientation, Vector2 area, Vector2 offset, Vector2 size)
        {
            Vector2 position = Vector2.Zero;

            if ((orientation & AnchorStyles.Left) != 0)
                position.X = offset.X;
            else if ((orientation & AnchorStyles.Right) != 0)
                position.X = area.X - size.X - offset.X;
            else
                position.X = (area.X - size.X) / 2f + offset.X;
            
            if ((orientation & AnchorStyles.Top) != 0)
                position.Y = offset.Y;
            else if ((orientation & AnchorStyles.Bottom) != 0)
                position.Y = area.Y - size.Y - offset.Y;
            else
                position.Y = (area.Y - size.Y) / 2f + offset.Y;

            return position;
        }
    }
}
