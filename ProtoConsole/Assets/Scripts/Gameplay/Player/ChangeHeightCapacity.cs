using System.Collections;
using UnityEngine;

public abstract class ChangeHeightCapacity : PlayerCapacity
{
    [Header("Parameters")]
    [SerializeField] private float movementHeight = 3;
    [SerializeField] private float movementDuration = 0.8f;
    [SerializeField] private AnimationCurve movementCurve = default;
    [SerializeField] private float planarMovementModifierCoef = 0.8f;

    protected override void LookToStartAction()
    {
        if (player.CanAddAltitudeModifier)
        {
            player.CanAddAltitudeModifier = false;
            player.MovementControlCoef = planarMovementModifierCoef;

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

        player.AltitudeModifier = 0;
        player.MovementControlCoef = 1;
        player.CanAddAltitudeModifier = true;

        yield return WaitForCooldown();

        currentAction = null;

        yield break;
    }
}