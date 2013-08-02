using System;

namespace Engine.Objects
{
    /// <summary>
    /// The queueable command types used by person objects.
    /// </summary>
    public enum Commands
    {
        Wait = 0,
        Animate = 1,
        FaceNorth = 2,
        FaceNorthEast = 3,
        FaceEast = 4,
        FceSouthEast = 5,
        FaceSouth = 6,
        FaceSouthWest = 7,
        FaceWest = 8,
        FaceNorthWest = 9,
        MoveNorth = 10,
        MoveEast = 11,
        MoveSouth = 12,
        MoveWest = 13,
    }
}

