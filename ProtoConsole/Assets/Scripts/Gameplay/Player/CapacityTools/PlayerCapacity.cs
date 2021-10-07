using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerCapacity : MonoBehaviour
{
    [SerializeField] private float cooldown = 0.8f;
    [SerializeField] private CapacityRenderer capacityConeSpawner = default;

    [Header("Parameters")]
    [SerializeField, Range(0, 1)] protected float planarMovementModifierCoef = 0.8f;

    protected abstract Capacity Type { get; }

    protected Player player = default;
    protected Coroutine currentAction = null;

    private Color coneColor = default;
    private Color coneColorOnCapacityUsed = default;

    protected virtual void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public virtual void TryToActivate(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
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

    protected Coroutine WaitForCooldown(MeshRenderer renderer) => StartCoroutine(Cooldown(renderer));

    protected void SetUsedColorOnRenderer(MeshRenderer renderer) => renderer.material.color = coneColorOnCapacityUsed;
    
    private IEnumerator Cooldown(MeshRenderer renderer)
    {
        Material capacityMaterial = renderer.material;
        AnimationCurve interpolation = capacityConeSpawner.ColorInterpolationCurveForCooldown;
        float elapsedTime = 0;

        while(elapsedTime <= cooldown)
        {
            elapsedTime += Time.deltaTime;
            capacityMaterial.color = Color.Lerp(coneColorOnCapacityUsed, coneColor, interpolation.Evaluate(elapsedTime / cooldown));

            yield return null;
        }

        yield break;
    }

    protected abstract void ClearCapacityEffects();

    public virtual void ResetCapacity()
    {
        StopAllCoroutines();
        enabled = false;
    }
}
