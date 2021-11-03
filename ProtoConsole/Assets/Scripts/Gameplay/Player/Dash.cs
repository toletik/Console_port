using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    UP = 0,
    RIGHT,
    DOWN,
    LEFT

    //UP = 0,
    //RIGHT = -90,
    //DOWN = 180,
    //LEFT = 90
}

public struct DirectionProperties
{
    public Direction dir;
    public bool isEnabled;
    public MeshRenderer renderer;
    public Vector2 directionVector;

    public DirectionProperties(Direction newDir, bool becomeEnabled, Vector2 newDirVec)
    {
        dir = newDir;
        isEnabled = becomeEnabled;
        renderer = null;
        directionVector = newDirVec;
    }

    public static float RetrieveRotation(Direction dir)
    {
        return dir switch
        {
            Direction.UP => 0,
            Direction.RIGHT => 3 * 90,
            Direction.DOWN => 2 * 90,
            Direction.LEFT => 1 * 90
        };
    }
}

public class DirectionList
{
    public DirectionProperties[] directions = new DirectionProperties[]
    {
        new DirectionProperties(Direction.UP, false, Vector2.up),
        new DirectionProperties(Direction.RIGHT, false, Vector2.right),
        new DirectionProperties(Direction.DOWN, false, Vector2.down),
        new DirectionProperties(Direction.LEFT, false, Vector2.left)
    };

    public ref DirectionProperties this[int i]
    {
        get
        {
            return ref directions[i];
        }
    }


    public ref DirectionProperties this[Direction direction]
    {
        get
        {
            return ref directions[(int)direction];
        }
    }
};


public class Dash : PlayerCapacity
{

    [SerializeField] private float dashDuration = 0.6f;
    [SerializeField] private float dashSpeed = 1.5f;
    [SerializeField] private AnimationCurve speedCurve = default;

    [Header("Feedback")]
    [SerializeField] private bool updateColorForAllDirectionOnUse = false;

    protected override Capacity Type => Capacity.DASH;

    DirectionList directionList = new DirectionList();

    private Direction currentDirection = default;
    private bool dashMovementFinished = true;

    protected override MeshRenderer SaveRenderer(MeshRenderer renderer)
    {
        return directionList[currentDirection].renderer = renderer;
    }

    public bool TryAddDirection(Direction direction)
    {
        ref DirectionProperties dirProps = ref directionList[direction];
        if (dirProps.isEnabled) 
            return false;
        
        return dirProps.isEnabled = true;
    }

    protected override bool TryToAssignCapacity()
    {
        return player.TryAddCapacity(Type, currentDirection);
    }

    protected override bool TryStartCapacity()
    {
        if (!directionList[currentDirection].isEnabled || player.IsUsingCapacity(Capacity.DIG)) 
            return false;

        player.MovementControlCoef = planarMovementModifierCoef;
        player.StartCapacity(Type);

        currentAction = player.StartCoroutine(ExecuteDash(directionList[currentDirection].directionVector));
        player.OnDeath += Player_OnDeath;

        SetUsedColorOnRenderer(directionList[currentDirection].renderer);

        return true;
    }

    private void Player_OnDeath(Player player, int possessedCollectibles = 0)
    {
        player.OnDeath -= Player_OnDeath;

        if (!dashMovementFinished)
        { 
            player.StopCoroutine(currentAction);

            EndCapacity();
            currentAction = null;
        }
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
        else
        {
            Debug.LogError("directionIndex should be a valid Direction enum (between 0 and 4).");
        }
    }
    private IEnumerator ExecuteDash(Vector2 direction)
    {
        dashMovementFinished = false;

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
                if (directionList[dir].renderer != default)
                    feedbackRenderers.Add(directionList[dir].renderer);
            }
        }
        else 
            feedbackRenderers.Add(directionList[currentDirection].renderer);

        yield return StartUpdateColor(feedbackRenderers);

        player.OnDeath -= Player_OnDeath;
        currentAction = null;
    }

    protected override void EndCapacity()
    {
        player.MovementControlCoef = 1;
        player.EndCapacity(Type);

        dashMovementFinished = true;
    }

    public override void ResetCapacity()
    {
        base.ResetCapacity();

        Array directions = Enum.GetValues(typeof(Direction));

        foreach (Direction direction in directions)
        {
            directionList[direction].isEnabled = false;
            directionList[direction].renderer = default;
        }
    }
}