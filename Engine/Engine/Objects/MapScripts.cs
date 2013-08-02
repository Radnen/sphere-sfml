using System;

namespace Engine.Objects
{
    /// <summary>
    /// Used by the map engine to run commands based on how the player
    /// enters or leaves the map.
    /// </summary>
    public enum MapScripts
    {
        Enter = 0,
        Leave = 1,
        LeaveNorth = 2,
        LeaveEast = 3,
        LeaveSouth = 4,
        LeaveWest = 5
    }
}

