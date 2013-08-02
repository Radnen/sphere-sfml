using System;

namespace Engine.Objects
{
    /// <summary>
    /// Used on surface objects to do interesting blitting.
    /// </summary>
    public enum BlendModes
    {
        Blend,
        Replace,
        RGBOnly,
        AlphaOnly,
        Add,
        Subtract,
        Multiply,
        Average
    }
}

