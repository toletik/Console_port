using System.Collections;
using UnityEngine;

public abstract class PlayerCapacity : MonoBehaviour
{
    [SerializeField] private float cooldown = 0.8f;

    protected Coroutine WaitForCooldown() => StartCoroutine(Cooldown());

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        yield break;
    }
}
