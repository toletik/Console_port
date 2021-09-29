using System.Collections;
using UnityEngine;

public abstract class PlayerCapacity : MonoBehaviour
{
    [SerializeField] private float cooldown = 0.8f;

    [Header("Controls")]
    [SerializeField] protected KeyCode activateKey = KeyCode.Keypad1;

    [Header("Parameters")]
    [SerializeField, Range(0, 1)] protected float planarMovementModifierCoef = 0.8f;

    protected Player player = default;
    protected Coroutine currentAction = null;

    protected virtual void OnEnable()
    {
        player = GetComponentInParent<Player>();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(activateKey) && currentAction == null)
        {
            LookToStartAction();
        }
    }

    protected abstract void LookToStartAction();

    protected Coroutine WaitForCooldown() => StartCoroutine(Cooldown());

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        yield break;
    }
}
