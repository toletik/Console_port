using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    UP = 0,
    RIGHT = -90,
    DOWN = 180,
    LEFT = 90
}



public class Dash : PlayerCapacity
{

    [SerializeField] private float dashDuration = 0.6f;
    [SerializeField] private float dashSpeed = 1.5f;
    [SerializeField] private AnimationCurve speedCurve = default;

    [Header("Feedback")]
    [SerializeField] private bool updateColorForAllDirectionOnUse = false;

    protected override Capacity Type => Capacity.DASH;

    private readonly Dictionary<Direction, bool> directionEnabled = new Dictionary<Direction, bool>()
    {
        { Direction.UP, false },
        { Direction.RIGHT, false },
        { Direction.DOWN, false },
        { Direction.LEFT, false }
    };

    private readonly Dictionary<Direction, MeshRenderer> allDirectionsRenderer = new Dictionary<Direction, MeshRenderer>()
    {
        { Direction.UP, default },
        { Direction.RIGHT, default },
        { Direction.DOWN, default },
        { Direction.LEFT, default }
    };


    private Direction currentDirection = default;


    protected override MeshRenderer SaveRenderer(MeshRenderer renderer)
    {
        return allDirectionsRenderer[currentDirection] = renderer;
    }

    public bool TryAddDirection(Direction direction)
    {
        if (directionEnabled[direction]) 
            return false;
        
        return directionEnabled[direction] = true;
    }

    protected override bool TryToAssignCapacity()
    {
        return player.TryAddCapacity(Type, currentDirection);
    }

    protected override bool TryStartCapacity()
    {
        if (!directionEnabled[currentDirection] || player.IsUsingCapacity(Capacity.DIG)) 
            return false;

        player.MovementControlCoef = planarMovementModifierCoef;
        player.StartCapacity(Type);

        currentAction = player.StartCoroutine(ExecuteDash(currentDirection switch
        {
            Direction.UP => new Vector2(0, 1),
            Direction.RIGHT => new Vector2(1, 0),
            Direction.DOWN => new Vector2(0, -1),
            Direction.LEFT => new Vector2(-1, 0),
            _ => new Vector2(0, 0)
        }));

        SetUsedColorOnRenderer(allDirectionsRenderer[currentDirection]);

        return true;
    }

    /// <param name="directionIndex"> -90 = right, 0 = up, 90 = left, 180 = down </param>
    public void ChooseDirection(int directionIndex)
    {
        if (directionIndex == (int)Direction.UP ||
            directionIndex == (int)Direction.RIGHT ||
            directionIndex == (int)Direction.DOWN ||
            directionIndex == (int)Direction.LEFT)
        {
            currentDirection = (Direction)directionIndex;
        }
    }
    private IEnumerator ExecuteDash(Vector2 direction)
    {

        for (float elapsedTime = 0f; elapsedTime <= dashDuration; elapsedTime += Time.deltaTime)
        {
            player.ExternalVelocity = direction * Mathf.LerpUnclamped(0, dashSpeed, speedCurve.Evaluate(elapsedTime / dashDuration));

            yield return null;
        }

        EndCapacity();

        List<MeshRenderer> feedbackRenderers = new List<MeshRenderer>();

        if (updateColorForAllDirectionOnUse)
        {
            Array directions = Enum.GetValues(typeof(Direction));

            foreach (Direction dir in directions)
            {
                if (allDirectionsRenderer[dir] != default)
                    feedbackRenderers.Add(allDirectionsRenderer[dir]);
            }
        }
        else feedbackRenderers.Add(allDirectionsRenderer[currentDirection]);

        yield return StartUpdateColor(feedbackRenderers);

        currentAction = null;
    }

    protected override void EndCapacity()
    {
        player.MovementControlCoef = 1;
        player.EndCapacity(Type);
    }

    public override void ResetCapacity()
    {
        base.ResetCapacity();

        Array directions = Enum.GetValues(typeof(Direction));

        foreach (Direction direction in directions)
        {
            directionEnabled[direction] = false;
            allDirectionsRenderer[direction] = default;
        }
    }
}