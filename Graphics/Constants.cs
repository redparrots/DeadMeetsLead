using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics
{
    public enum Orientation
    {
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        Center
    }

    public enum TextOverflow 
    { 
        Ignore, 
        Hide, 
        Truncate 
    }

    public enum MouseState { Out, Over, Down, Up }

    public enum OrientationRelation { Relative, Absolute }

    public enum WindowMode
    {
        [Common.ResourceStringAttribute("VideoEnumWindowed")]
        Windowed,
        [Common.ResourceStringAttribute("VideoEnumFullscreen")]
        Fullscreen,
        [Common.ResourceStringAttribute("VideoEnumFullscreenWindowed")]
        FullscreenWindowed
    }

    public enum MessageType
    {
        MouseDown,
        MouseUp,
        Click,
        MouseClick,
        Update,
        MouseEnter,
        MouseLeave,
        MouseMove,
        MouseWheel,
        Resize,
        KeyDown,
        KeyUp,
        KeyPress
    }

    public enum Direct3DVersion { Direct3D9, Direct3D10 }

    public enum MeshType { Indexed, TriangleStrip, PointList, LineStrip }

    [Flags]
    public enum Facings { Frontside = 1, Backside = 2 }

    public enum AnimationTimeType
    {
        Speed,
        Length
    }

    public enum AutoSizeMode
    {
        None,
        Full,
        Vertical,
        Horizontal,
        /// <summary>
        /// Uses at the MaxSize property
        /// </summary>
        RestrictedFull
    }
}
