using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : PlayerCapacity
{
    [SerializeField] private float dashDuration = 0.6f;
    [SerializeField] private float dashSpeed = 1.5f;
    [SerializeField] private AnimationCurve speedCurve = default;

    private readonly Dictionary<Direction, bool> directionEnabled = new Dictionary<Direction, bool>()
    {
        { Direction.UP, false },
        { Direction.RIGHT, false },
        { Direction.DOWN, false },
        { Direction.LEFT, false }
    };

    private Direction currentDirection = default;

    public bool TryAddDirection(Direction direction)
    {
        if (directionEnabled[direction]) 
            return false;
        
        return directionEnabled[direction] = true;
    }

    protected override bool TryToAssignCapacity()
    {
        return player.TryAddCapacity(Capacity.DASH, currentDirection);
    }

    protected override void LookToStartAction()
    {
        if (!directionEnabled[currentDirection] || player.IsUsingCapacity(Capacity.DIG)) return;

        player.MovementControlCoef = planarMovementModifierCoef;
        player.StartCapacity(Capacity.DASH);

        currentAction = StartCoroutine(ExecuteDash(currentDirection switch
        {
            Direction.UP => new Vector2(0, 1),
            Direction.RIGHT => new Vector2(1, 0),
            Direction.DOWN => new Vector2(0, -1),
            Direction.LEFT => new Vector2(-1, 0), 
            _ => new Vector2(0, 1)
        }));
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
        float elapsedTime = 0;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            player.ExternalVelocity = direction * Mathf.LerpUnclamped(0, dashSpeed, speedCurve.Evaluate(elapsedTime / dashDuration));

            yield return null;
        }

        ClearCapacityEffects();

        yield return WaitForCooldown();

        currentAction = null;
        yield break;
    }

    protected override void ClearCapacityEffects()
    {
        player.MovementControlCoef = 1;
        player.EndCapacity(Capacity.DASH);
    }
}