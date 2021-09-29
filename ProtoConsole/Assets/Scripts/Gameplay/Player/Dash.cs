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

    public bool TryAddDirection(Direction direction)
    {
        if (directionEnabled[direction]) 
            return false;
        
        return directionEnabled[direction] = true;
    }

    protected override void LookToStartAction()
    {
        player.MovementControlCoef = planarMovementModifierCoef;
        currentAction = StartCoroutine(ExecuteDash(new Vector2(-1, 0)));
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

        player.MovementControlCoef = 1;
        yield return WaitForCooldown();

        currentAction = null;
        yield break;
    }
}