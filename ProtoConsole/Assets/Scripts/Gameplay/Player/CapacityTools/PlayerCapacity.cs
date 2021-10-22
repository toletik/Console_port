using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
            Debug.LogError(gameObject.name + " click = " + player.AssignationMode);

            if (player.AssignationMode) TryToAssignCapacity();
            else if (isActiveAndEnabled && currentAction == null) LookToStartAction();
        }
    }

    public Transform CreateRenderer(Transform rendererParent)
    {
        Transform renderer = capacityConeSpawner.CreateRendererForCapacity(Type, rendererParent);

        coneColor = SaveRenderer(renderer.GetComponentInChildren<MeshRenderer>()).material.color;
        coneColorOnCapacityUsed = capacityConeSpawner.GetUsedMaterialColorForCapacity(Type);

        return renderer;
    }

    protected abstract MeshRenderer SaveRenderer(MeshRenderer renderer);
    protected abstract bool TryToAssignCapacity();
    protected abstract bool LookToStartAction();

    protected Coroutine WaitForCooldown(List<MeshRenderer> renderers) => player.StartCoroutine(Cooldown(renderers));

    protected void SetUsedColorOnRenderer(MeshRenderer renderer) => renderer.material.color = coneColorOnCapacityUsed;
    
    private IEnumerator Cooldown(List<MeshRenderer> renderer)
    {
        List<Material> capacityMaterials = new List<Material>();
        AnimationCurve interpolation = capacityConeSpawner.ColorInterpolationCurveForCooldown;
        Color color;
        int numberOfMaterials = renderer.Count;
        float elapsedTime = 0;

        for (int i = 0; i < numberOfMaterials; i++)
        {
            capacityMaterials.Add(renderer[i].material);
        }

        while(elapsedTime <= cooldown)
        {
            elapsedTime += Time.deltaTime;
            color = Color.Lerp(coneColorOnCapacityUsed, coneColor, interpolation.Evaluate(elapsedTime / cooldown));

            for (int i = 0; i < numberOfMaterials; i++)
            {
                capacityMaterials[i].color = color;
            }

            yield return null;
        }

        yield break;
    }

    protected abstract void ClearCapacityEffects();

    public virtual void ResetCapacity()
    {
        if (currentAction != null)
        {
            player.StopCoroutine(currentAction);
            currentAction = null;
        }

        enabled = false;
    }
}
