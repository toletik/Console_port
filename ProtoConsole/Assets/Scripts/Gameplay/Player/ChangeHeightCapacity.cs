using System.Collections;
using UnityEngine;

public abstract class ChangeHeightCapacity : PlayerCapacity
{
    [SerializeField] private float movementHeight = 3;
    [SerializeField] private float movementDuration = 0.8f;
    [SerializeField] private AnimationCurve movementCurve = default;

    protected abstract Capacity Type { get; }

    protected override void LookToStartAction()
    {
        if (player.CanAddAltitudeModifier)
        {
            player.CanAddAltitudeModifier = false;
            player.MovementControlCoef = planarMovementModifierCoef;
            player.StartCapacity(Type);

            currentAction = StartCoroutine(UpdateHeight());
        }
    }

    private IEnumerator UpdateHeight()
    {
        float elapsedTime = 0;

        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;
            player.AltitudeModifier = Mathf.LerpUnclamped(0, movementHeight, movementCurve.Evaluate(elapsedTime / movementDuration));

            yield return null;
        }

        ClearCapacityEffects();

        yield return WaitForCooldown();

        currentAction = null;
        yield break;
    }

    protected override void ClearCapacityEffects()
    {
        player.AltitudeModifier = 0;
        player.MovementControlCoef = 1;
        player.CanAddAltitudeModifier = true;
        player.EndCapacity(Type);
    }
}