using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerCapacity : MonoBehaviour
{
    [SerializeField] private float cooldown = 0.8f;

    [Header("Parameters")]
    [SerializeField, Range(0, 1)] protected float planarMovementModifierCoef = 0.8f;

    protected Player player = default;
    protected Coroutine currentAction = null;

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

    protected abstract bool TryToAssignCapacity();
    protected abstract void LookToStartAction();

    protected Coroutine WaitForCooldown() => StartCoroutine(Cooldown());

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        yield break;
    }

    protected abstract void ClearCapacityEffects();
}
