using System.Collections.Generic;
using UnityEngine;

public class Dash : PlayerCapacity
{
    private readonly Dictionary<Direction, bool> directionEnabled = new Dictionary<Direction, bool>()
    {
        { Direction.UP, false },
        { Direction.RIGHT, false },
        { Direction.DOWN, false },
        { Direction.LEFT, false }
    };

    public bool TryAddDirection(Direction direction)
    {
        if (directionEnabled[direction]) 
            return false;
        
        return directionEnabled[direction] = true;
    }
}