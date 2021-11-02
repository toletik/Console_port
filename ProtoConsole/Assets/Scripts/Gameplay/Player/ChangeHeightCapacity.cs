using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChangeHeightCapacity : PlayerCapacity
{
    [SerializeField] private float movementHeight = 3;
    [SerializeField] private float movementDuration = 0.8f;
    [SerializeField] private AnimationCurve movementCurve = default;

    protected new MeshRenderer renderer = default;


    protected override MeshRenderer SaveRenderer(MeshRenderer _renderer)
    {
        return renderer = _renderer;
    }

    protected override bool TryToAssignCapacity()
    {
        return player.TryAddCapacity(Type);
    }

    protected override bool TryStartCapacity()
    {
        if (player.CanAddAltitudeModifier)
        {
            player.CanAddAltitudeModifier = false;
            player.MovementControlCoef = planarMovementModifierCoef;
            player.StartCapacity(Type);

            currentAction = player.StartCoroutine(UpdateHeight());

            SetUsedColorOnRenderer(renderer);

            return true;
        }

        return false;
    }

    private IEnumerator UpdateHeight()
    {

        for (float elapsedTime = 0f; elapsedTime <= movementDuration; elapsedTime += Time.deltaTime)
        {
            player.AltitudeModifier = Mathf.LerpUnclamped(0, movementHeight, movementCurve.Evaluate(elapsedTime / movementDuration));
            yield return null;
        }

        EndCapacity();

        yield return StartUpdateColor(new List<MeshRenderer>() { renderer });

        currentAction = null;
        yield break;
    }

    protected override void EndCapacity()
    {
        player.AltitudeModifier = 0;
        player.MovementControlCoef = 1;
        player.CanAddAltitudeModifier = true;
        player.EndCapacity(Type);
    }

    public override void ResetCapacity()
    {
        base.ResetCapacity();
        renderer = default;
    }
}