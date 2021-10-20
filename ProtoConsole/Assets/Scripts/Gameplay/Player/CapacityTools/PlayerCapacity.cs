using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public enum Capacity
{
    NONE,
    DIG,
    DASH,
    JUMP,
    DASH_AND_JUMP = 5
}

public abstract class PlayerCapacity : MonoBehaviour
{
    [SerializeField] protected Player player = default;
    [SerializeField] private float cooldown = 0.8f;
    [SerializeField] private CapacityRenderer capacityConeSpawner = default;

    [Header("Parameters")]
    [SerializeField, Range(0, 1)] protected float planarMovementModifierCoef = 0.8f;

    protected abstract Capacity Type { get; }

    protected Coroutine currentAction = null;

    private Color coneColor = default;
    private Color coneColorOnCapacityUsed = default;


    public virtual void TryToActivate(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            if (player.AssignationMode) 
                TryToAssignCapacity();
            else if (isActiveAndEnabled && currentAction == null)
                TryStartCapacity();
        }
    }
    public Transform CreateRenderer(Transform rendererParent)
    {
        Transform renderer = capacityConeSpawner.CreateRendererForCapacity(Type, rendererParent);

        coneColor = SaveRenderer(renderer.GetComponentInChildren<MeshRenderer>()).material.color;
        coneColorOnCapacityUsed = capacityConeSpawner.GetUsedMaterialColorForCapacity(Type);

        return renderer;
    }
    protected Coroutine StartUpdateColor(List<MeshRenderer> renderers) => player.StartCoroutine(UpdateColor(renderers));
    protected void SetUsedColorOnRenderer(MeshRenderer renderer) => renderer.material.color = coneColorOnCapacityUsed;
    public virtual void ResetCapacity()
    {
        if (currentAction != null)
        {
            player.StopCoroutine(currentAction);
            currentAction = null;
        }

        enabled = false;
    }
    private IEnumerator UpdateColor(List<MeshRenderer> renderers)
    {
        AnimationCurve interpolation = capacityConeSpawner.ColorInterpolationCurveForCooldown;

        for (float elapsedTime = 0f; elapsedTime <= cooldown; elapsedTime += Time.deltaTime)
        {
            Color color = Color.Lerp(coneColorOnCapacityUsed, coneColor, interpolation.Evaluate(elapsedTime / cooldown));

            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].material.color = color;
            }

            yield return null;
        }

    }
 

    //Abstracts
    protected abstract MeshRenderer SaveRenderer(MeshRenderer renderer);
    protected abstract bool TryToAssignCapacity();
    protected abstract bool TryStartCapacity();
    protected abstract void EndCapacity();

}
