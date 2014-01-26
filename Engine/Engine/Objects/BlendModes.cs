using System;

namespace Engine.Objects
{
    /// <summary>
    /// Used on surface objects to do interesting blitting.
    /// </summary>
    public enum BlendModes
    {
        Blend = 0,
        Replace = 1,
        RGBOnly = 2,
        AlphaOnly = 3,
        Add = 4,
        Subtract = 5,
        Multiply = 6,
        Average = 7
    }
}
